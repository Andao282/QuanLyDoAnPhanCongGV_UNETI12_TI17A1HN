// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 3 - Quản lý sinh viên (mục 7.1 đề tài)
// Tìm kiếm / lọc / sắp xếp / phân trang danh sách sinh viên.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class SinhViensController : Controller
    {
        private const int SoBanMoiTrang = 10;

        private readonly AppDbContext _context;

        public SinhViensController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Danh sách sinh viên có tìm kiếm - lọc - sắp xếp - phân trang
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai,
            string? sapXep,
            int trang = 1)
        {
            // Where(): tìm theo mã, họ tên, lớp và email
            var danhSach = _context.SinhViens.AsNoTracking().Include(s => s.TaiKhoan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                danhSach = danhSach.Where(s =>
                    s.MaSinhVien.Contains(kw)
                    || s.HoTen.Contains(kw)
                    || (s.Lop != null && s.Lop.Contains(kw))
                    || (s.Email != null && s.Email.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                danhSach = danhSach.Where(s => s.TrangThai == trangThai);
            }

            // OrderBy(): sắp xếp theo lựa chọn
            danhSach = sapXep switch
            {
                "TenZ" => danhSach.OrderByDescending(s => s.HoTen),
                "MaGiamDan" => danhSach.OrderBy(s => s.MaSinhVien),
                _ => danhSach.OrderBy(s => s.HoTen)
            };

            int tongSoBan = await danhSach.CountAsync();

            // Skip()/Take(): phân trang bằng truy vấn EF Core, không nạp hết về bộ nhớ
            int trangHienTai = trang < 1 ? 1 : trang;
            int tongSoTrang = (int)Math.Ceiling(tongSoBan / (double)SoBanMoiTrang);
            if (tongSoTrang > 0 && trangHienTai > tongSoTrang)
            {
                trangHienTai = tongSoTrang;
            }

            var dsHienThi = await danhSach
                .Skip((trangHienTai - 1) * SoBanMoiTrang)
                .Take(SoBanMoiTrang)
                .ToListAsync();

            ViewData["TuKhoa"] = tuKhoa;
            ViewData["TrangThai"] = trangThai;
            ViewData["SapXep"] = sapXep;
            ViewData["TrangHienTai"] = trangHienTai;
            ViewData["TongSoTrang"] = tongSoTrang;
            ViewData["TongSoBan"] = tongSoBan;
            ViewData["DanhSachTrangThai"] = TrangThaiHeThong.SinhVien.TatCa;

            return View(dsHienThi);
        }

        // GET: Chi tiết sinh viên kèm lịch sử đăng ký của chính sinh viên đó
        public async Task<IActionResult> Details(string? maSinhVien)
        {
            if (string.IsNullOrEmpty(maSinhVien))
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.AsNoTracking()
                .Include(s => s.TaiKhoan)
                .FirstOrDefaultAsync(s => s.MaSinhVien == maSinhVien);

            if (sinhVien == null)
            {
                return NotFound();
            }

            ViewData["DangKys"] = await _context.DangKyDeTais.AsNoTracking()
                .Include(d => d.DeTai)
                .Where(d => d.MaSinhVien == maSinhVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(sinhVien);
        }

        // GET: Mở form thêm
        public IActionResult Create() => View(new SinhVien());

        // POST: Thêm sinh viên - kiểm tra trùng bằng LINQ (mục 12)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            sinhVien.MaSinhVien = sinhVien.MaSinhVien?.Trim() ?? string.Empty;
            sinhVien.HoTen = sinhVien.HoTen?.Trim() ?? string.Empty;

            if (ModelState.IsValid && await KiemTraTrungMaAsync(sinhVien.MaSinhVien, null))
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                TempData["ThongBao"] = $"Đã thêm sinh viên {sinhVien.HoTen}.";
                return RedirectToAction(nameof(Index));
            }

            return View(sinhVien);
        }

        // GET: Mở form sửa
        public async Task<IActionResult> Edit(string? maSinhVien)
        {
            if (string.IsNullOrEmpty(maSinhVien))
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.AsNoTracking()
                .FirstOrDefaultAsync(s => s.MaSinhVien == maSinhVien);

            return sinhVien == null ? NotFound() : View(sinhVien);
        }

        // POST: Sửa sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SinhVien sinhVien)
        {
            if (!ModelState.IsValid)
            {
                return View(sinhVien);
            }

            var svTrongDb = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaSinhVien == sinhVien.MaSinhVien);
            if (svTrongDb == null)
            {
                return NotFound();
            }

            // Không cho đổi khóa chính
            if (!string.Equals(sinhVien.MaSinhVien, svTrongDb.MaSinhVien))
            {
                return NotFound();
            }

            svTrongDb.HoTen = sinhVien.HoTen?.Trim() ?? string.Empty;
            svTrongDb.NgaySinh = sinhVien.NgaySinh;
            svTrongDb.Lop = sinhVien.Lop;
            svTrongDb.Khoa = sinhVien.Khoa;
            svTrongDb.ChuyenNganh = sinhVien.ChuyenNganh;
            svTrongDb.Email = sinhVien.Email;
            svTrongDb.SoDienThoai = sinhVien.SoDienThoai;
            svTrongDb.TrangThai = sinhVien.TrangThai;

            await _context.SaveChangesAsync();
            TempData["ThongBao"] = $"Đã cập nhật thông tin sinh viên {svTrongDb.HoTen}.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Mã sinh viên bắt buộc và không trùng (mục 7.1) - kiểm tra bằng LINQ, không dùng được kiểm tra Data Annotation.
        /// </summary>
        private async Task<bool> KiemTraTrungMaAsync(string maSinhVien, string? maDangSoSanh)
        {
            bool trung = await _context.SinhViens.AsNoTracking()
                .AnyAsync(s => s.MaSinhVien == maSinhVien && s.MaSinhVien != maDangSoSanh);

            if (trung)
            {
                ModelState.AddModelError(nameof(SinhVien.MaSinhVien), "Mã sinh viên đã tồn tại, không được trùng.");
            }

            return !trung;
        }
    }
}