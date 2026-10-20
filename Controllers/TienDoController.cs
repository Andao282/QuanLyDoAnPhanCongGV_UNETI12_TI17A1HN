// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 4 - Theo dõi tiến độ thực hiện đề tài (mục 8.6 đề tài)
// Cho phép cập nhật trạng thái thực hiện của một đăng ký: Chưa bắt đầu -> Đang thực hiện -> Hoàn thành.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class TienDoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NghipVuService _nghipVu;

        public TienDoController(AppDbContext context, NghipVuService nghipVu)
        {
            _context = context;
            _nghipVu = nghipVu;
        }

        // GET: Danh sách đăng ký đã duyệt kèm trạng thái thực hiện, tìm kiếm và lọc
        public async Task<IActionResult> Index(string? tuKhoa, string? trangThai, bool? thieuGv)
        {
            var danhSach = _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai).ThenInclude(t => t!.LinhVuc)
                .Where(d => d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                danhSach = danhSach.Where(d =>
                    d.SinhVien.HoTen.Contains(kw)
                    || d.SinhVien.MaSinhVien.Contains(kw)
                    || (d.DeTai != null && d.DeTai.TenDeTai.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                danhSach = danhSach.Where(d => d.TrangThaiThucHien == trangThai);
            }

            // Lọc các đăng ký đã duyệt nhưng chưa có giảng viên (mục 8.5)
            if (thieuGv == true)
            {
                danhSach = danhSach.Where(d => !_context.PhanCongHuongDans.AsNoTracking()
                    .Any(p => p.MaDangKy == d.MaDangKy
                              && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan));
            }

            var ds = await danhSach
                .OrderBy(d => d.TrangThaiThucHien)
                .ThenBy(d => d.SinhVien.HoTen)
                .ToListAsync();

            // GVHD hiện tại của từng đăng ký, gom bằng LINQ
            var maDangKys = ds.Select(d => d.MaDangKy).ToList();
            var gvHienTai = await _context.PhanCongHuongDans.AsNoTracking()
                .Include(p => p.GiangVien)
                .Where(p => maDangKys.Contains(p.MaDangKy)
                            && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
                .ToDictionaryAsync(p => p.MaDangKy, p => p.GiangVien!.HoTen);

            ViewData["GvHienTai"] = gvHienTai;
            ViewData["TuKhoa"] = tuKhoa;
            ViewData["TrangThai"] = trangThai;
            ViewData["ThieuGv"] = thieuGv;

            return View(ds);
        }

        // POST: Cập nhật trạng thái thực hiện (mục 8.6)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(int maDangKy, string trangThaiThucHien)
        {
            var dangKy = await _context.DangKyDeTais.FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);
            if (dangKy == null)
            {
                return NotFound();
            }

            // Chỉ nhận trạng thái nằm trong tập giá trị quy định ở mục 9.1
            if (!TrangThaiHeThong.TrangThaiThucHien.TatCa.Contains(trangThaiThucHien))
            {
                TempData["Loi"] = "Trạng thái thực hiện không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            dangKy.TrangThaiThucHien = trangThaiThucHien;
            await _context.SaveChangesAsync();

            // Đề tài đã đủ số lượng thì đồng bộ lại trạng thái mở/đóng cho nhất quán
            await _nghipVu.DongBoTrangThaiDeTaiAsync(dangKy.MaDeTai);

            TempData["ThongBao"] = $"Đã cập nhật tiến độ: {TrangThaiHeThong.TrangThaiThucHien.HienThi(trangThaiThucHien)}.";
            return RedirectToAction(nameof(Index));
        }
    }
}