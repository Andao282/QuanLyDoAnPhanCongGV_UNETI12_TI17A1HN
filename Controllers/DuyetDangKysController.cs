// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 4 - Duyệt / từ chối đăng ký đề tài (mục 8.1 đề tài)
// Danh sách có tìm kiếm và lọc, duyệt/từ chối có ghi chú, có kiểm tra lại điều kiện bằng LINQ.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class DuyetDangKysController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NghipVuService _nghipVu;

        public DuyetDangKysController(AppDbContext context, NghipVuService nghipVu)
        {
            _context = context;
            _nghipVu = nghipVu;
        }

        // GET: Danh sách đăng ký chờ duyệt, tìm kiếm và lọc theo trạng thái / lĩnh vực / đề tài
        public async Task<IActionResult> Index(string? tuKhoa, string? trangThai, int? maLinhVuc)
        {
            var danhSach = _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai).ThenInclude(t => t!.LinhVuc)
                .AsQueryable();

            // Mặc định mở màn hình ở tab "Chờ duyệt" vì đó là việc cần làm
            trangThai ??= TrangThaiHeThong.DangKyDeTai.ChoDuyet;

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
                danhSach = danhSach.Where(d => d.TrangThai == trangThai);
            }

            if (maLinhVuc != null)
            {
                danhSach = danhSach.Where(d => d.DeTai != null && d.DeTai.MaLinhVuc == maLinhVuc);
            }

            var ds = await danhSach
                .OrderBy(d => d.TrangThai) // Chờ duyệt (Theo thứ tự chuỗi: ChoDuyet) lên trên
                .ThenBy(d => d.NgayDangKy)
                .ToListAsync();

            // Số chỗ còn của từng đề tài phải tính lại tại thời điểm duyệt (mục 7.6)
            var soChoCon = await _nghipVu.TinhSoChoConNhieuAsync(ds.Select(d => d.MaDeTai));
            ViewData["SoChoCon"] = soChoCon;

            ViewData["TuKhoa"] = tuKhoa;
            ViewData["TrangThai"] = trangThai;
            ViewData["MaLinhVuc"] = maLinhVuc;
            ViewData["DanhSachLinhVuc"] = await _context.LinhVucs.AsNoTracking().OrderBy(l => l.TenLinhVuc).ToListAsync();

            return View(ds);
        }

        // GET: Duyệt một đăng ký
        public async Task<IActionResult> Duyet(int? maDangKy)
        {
            var dangKy = await _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai).ThenInclude(t => t!.LinhVuc)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dangKy == null)
            {
                return NotFound();
            }

            // Hiển thị số chỗ còn để Admin không duyệt nhầm đề tài đã đủ
            ViewData["SoChoCon"] = await _nghipVu.TinhSoChoConAsync(dangKy.MaDeTai);
            return View(dangKy);
        }

        // POST: Duyệt đăng ký (mục 8.1)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(int maDangKy, string? ghiChu)
        {
            var dangKy = await _context.DangKyDeTais.FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);
            if (dangKy == null)
            {
                return NotFound();
            }

            // Mục 7.6: phải kiểm tra lại toàn bộ điều kiện tại thời điểm duyệt,
            // không được tin vào kết quả kiểm tra lúc sinh viên đăng ký.
            var kiemTra = await _nghipVu.KiemTraDieuKienDuyetAsync(dangKy);
            if (!kiemTra.HopLe)
            {
                TempData["Loi"] = kiemTra.ThongBao;
                return RedirectToAction(nameof(Index));
            }

            dangKy.TrangThai = TrangThaiHeThong.DangKyDeTai.DaDuyet;
            dangKy.GhiChuDuyet = string.IsNullOrWhiteSpace(ghiChu) ? "Hợp lệ, đề nghị triển khai." : ghiChu.Trim();
            await _context.SaveChangesAsync();

            // Duyệt xong có thể đã đủ số lượng, cần cập nhật lại trạng thái đề tài
            await _nghipVu.DongBoTrangThaiDeTaiAsync(dangKy.MaDeTai);

            TempData["ThongBao"] = $"Đã duyệt đăng ký đề tài của sinh viên {dangKy.MaSinhVien}.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Từ chối một đăng ký
        public async Task<IActionResult> TuChoi(int? maDangKy)
        {
            var dangKy = await _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            return dangKy == null ? NotFound() : View(dangKy);
        }

        // POST: Từ chối đăng ký (mục 8.1)
        // Từ chối phải có lý do để sinh viên biết mình cần bổ sung gì.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(int maDangKy, string? lyDoTuChoi)
        {
            var dangKy = await _context.DangKyDeTais.FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);
            if (dangKy == null)
            {
                return NotFound();
            }

            if (dangKy.TrangThai != TrangThaiHeThong.DangKyDeTai.ChoDuyet)
            {
                TempData["Loi"] = "Đăng ký này đã được xử lý trước đó.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                TempData["Loi"] = "Vui lòng nhập lý do từ chối để sinh viên nắm được nguyên nhân.";
                return RedirectToAction(nameof(TuChoi), new { maDangKy });
            }

            dangKy.TrangThai = TrangThaiHeThong.DangKyDeTai.TuChoi;
            dangKy.GhiChuDuyet = lyDoTuChoi.Trim();
            await _context.SaveChangesAsync();

            // Từ chối không làm thay đổi số chỗ, nhưng vẫn đồng bộ cho chắc chắn
            await _nghipVu.DongBoTrangThaiDeTaiAsync(dangKy.MaDeTai);

            TempData["ThongBao"] = "Đã từ chối đăng ký và ghi nhận lý do.";
            return RedirectToAction(nameof(Index));
        }
    }
}