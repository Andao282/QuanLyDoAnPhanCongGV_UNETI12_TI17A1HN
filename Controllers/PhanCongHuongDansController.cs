// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 4 - Phân công / thay đổi / kết thúc giảng viên hướng dẫn (mục 8.3 - 8.5)
// Quy tắc mục 8.4 (giới hạn tải) và mục 8.3 (đúng lĩnh vực) do NghipVuService kiểm tra bằng LINQ.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class PhanCongHuongDansController : Controller
    {
        private readonly AppDbContext _context;
        private readonly NghipVuService _nghipVu;

        public PhanCongHuongDansController(AppDbContext context, NghipVuService nghipVu)
        {
            _context = context;
            _nghipVu = nghipVu;
        }

        // GET: Danh sách phân công, tìm kiếm và lọc theo giảng viên / lĩnh vực / trạng thái
        public async Task<IActionResult> Index(string? tuKhoa, string? maGiangVien, string? trangThai, int? maLinhVuc)
        {
            var danhSach = _context.PhanCongHuongDans
                .AsNoTracking()
                .Include(p => p.GiangVien)
                .Include(p => p.DangKyDeTai).ThenInclude(d => d!.DeTai).ThenInclude(t => t!.LinhVuc)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                danhSach = danhSach.Where(p =>
                    p.GiangVien.HoTen.Contains(kw)
                    || p.GiangVien.MaGiangVien.Contains(kw)
                    || p.DangKyDeTai.SinhVien.HoTen.Contains(kw)
                    || p.DangKyDeTai.SinhVien.MaSinhVien.Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(maGiangVien))
            {
                danhSach = danhSach.Where(p => p.MaGiangVien == maGiangVien);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                danhSach = danhSach.Where(p => p.TrangThai == trangThai);
            }

            if (maLinhVuc != null)
            {
                danhSach = danhSach.Where(p => p.DangKyDeTai.DeTai != null
                                            && p.DangKyDeTai.DeTai.MaLinhVuc == maLinhVuc);
            }

            var ds = await danhSach
                .OrderByDescending(p => p.NgayBatDau)
                .ToListAsync();

            // Mục 8.4: tải hướng dẫn hiện tại của từng giảng viên, tính bằng LINQ
            ViewData["TaiHienTai"] = await _nghipVu.TaiHienTaiNhieuGiangVienAsync();
            ViewData["TuKhoa"] = tuKhoa;
            ViewData["MaGiangVien"] = maGiangVien;
            ViewData["TrangThai"] = trangThai;
            ViewData["MaLinhVuc"] = maLinhVuc;
            ViewData["DanhSachGiangVien"] = await _context.GiangViens.AsNoTracking()
                .OrderBy(g => g.HoTen).ToListAsync();
            ViewData["DanhSachLinhVuc"] = await _context.LinhVucs.AsNoTracking()
                .OrderBy(l => l.TenLinhVuc).ToListAsync();

            return View(ds);
        }

        /// <summary>
        /// Danh sách đăng ký Đã duyệt nhưng chưa có giảng viên - nguồn dữ liệu cho màn hình phân công (mục 8.3).
        /// </summary>
        public async Task<IActionResult> ChoPhanCong(string? tuKhoa, int? maLinhVuc)
        {
            var danhSach = _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai).ThenInclude(t => t!.LinhVuc)
                .Where(d => d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet)
                // Chỉ lấy đăng ký chưa có phân công nào ở trạng thái Đang hướng dẫn
                .Where(d => !_context.PhanCongHuongDans.AsNoTracking()
                    .Any(p => p.MaDangKy == d.MaDangKy
                              && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan));

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                danhSach = danhSach.Where(d =>
                    d.SinhVien.HoTen.Contains(kw)
                    || d.SinhVien.MaSinhVien.Contains(kw)
                    || (d.DeTai != null && d.DeTai.TenDeTai.Contains(kw)));
            }

            if (maLinhVuc != null)
            {
                danhSach = danhSach.Where(d => d.DeTai != null && d.DeTai.MaLinhVuc == maLinhVuc);
            }

            var ds = await danhSach.OrderBy(d => d.NgayDangKy).ToListAsync();

            // Chỉ hiện giảng viên đúng lĩnh vực và còn dưới giới hạn tải (mục 8.3, 8.4)
            var taiHienTai = await _nghipVu.TaiHienTaiNhieuGiangVienAsync();
            var gvDuDieuKien = await _context.GiangViens.AsNoTracking()
                .Where(g => g.TrangThai == TrangThaiHeThong.GiangVien.DangCongTac)
                .ToListAsync();
            gvDuDieuKien.RemoveAll(g => taiHienTai.TryGetValue(g.MaGiangVien, out var s)
                                    && s >= g.SoSinhVienHuongDanToiDa);

            ViewData["DanhSachGv"] = gvDuDieuKien;
            ViewData["TaiHienTai"] = taiHienTai;
            ViewData["TuKhoa"] = tuKhoa;
            ViewData["MaLinhVuc"] = maLinhVuc;
            ViewData["DanhSachLinhVuc"] = await _context.LinhVucs.AsNoTracking()
                .OrderBy(l => l.TenLinhVuc).ToListAsync();

            return View(ds);
        }

        // GET: Mở form phân công cho một đăng ký
        public async Task<IActionResult> PhanCong(int? maDangKy)
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

            await NapDanhSachGiangVienAsync(dangKy.MaDeTai, null);
            return View(dangKy);
        }

        // POST: Lưu phân công giảng viên hướng dẫn (mục 8.3)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanCong(int maDangKy, string maGiangVien, DateTime ngayBatDau, string? ghiChu)
        {
            var dangKy = await _context.DangKyDeTais.AsNoTracking().FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);
            if (dangKy == null)
            {
                return NotFound();
            }

            // Mục 8.3 và 8.4: kiểm tra lại điều kiện bằng LINQ trước khi lưu
            var kiemTra = await _nghipVu.KiemTraDieuKienPhanCongAsync(maDangKy, maGiangVien);
            if (!kiemTra.HopLe)
            {
                TempData["Loi"] = kiemTra.ThongBao;
                return RedirectToAction(nameof(ChoPhanCong));
            }

            var phanCong = new PhanCongHuongDan
            {
                MaDangKy = maDangKy,
                MaGiangVien = maGiangVien,
                NgayBatDau = ngayBatDau.Date,
                NgayKetThuc = null,
                TrangThai = TrangThaiHeThong.PhanCongHuongDan.DangHuongDan,
                GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim()
            };

            _context.PhanCongHuongDans.Add(phanCong);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đã phân công giảng viên hướng dẫn thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Mở form thay đổi giảng viên hướng dẫn (mục 8.5)
        // Phân công cũ được giữ nguyên và chuyển sang trạng thái "Đã thay đổi" để không mất lịch sử.
        public async Task<IActionResult> ThayDoiGiangVien(int? maPhanCong)
        {
            var phanCong = await _context.PhanCongHuongDans
                .AsNoTracking()
                .Include(p => p.GiangVien)
                .Include(p => p.DangKyDeTai).ThenInclude(d => d!.DeTai).ThenInclude(t => t!.LinhVuc)
                .FirstOrDefaultAsync(p => p.MaPhanCong == maPhanCong);

            if (phanCong == null)
            {
                return NotFound();
            }

            if (phanCong.TrangThai != TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
            {
                TempData["Loi"] = "Chỉ phân công đang ở trạng thái Đang hướng dẫn mới được thay đổi giảng viên.";
                return RedirectToAction(nameof(Index));
            }

            await NapDanhSachGiangVienAsync(phanCong.DangKyDeTai!.MaDeTai, phanCong.MaGiangVien);
            ViewData["PhanCong"] = phanCong;
            ViewData["MaPhanCong"] = phanCong.MaPhanCong;
            return View(phanCong.DangKyDeTai);
        }

        // POST: Lưu thay đổi giảng viên hướng dẫn (mục 8.5)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThayDoiGiangVien(int maPhanCong, string maGiangVien, DateTime ngayBatDau, string? ghiChu)
        {
            var phanCongCu = await _context.PhanCongHuongDans
                .Include(p => p.DangKyDeTai)
                .FirstOrDefaultAsync(p => p.MaPhanCong == maPhanCong);

            if (phanCongCu == null)
            {
                return NotFound();
            }

            if (phanCongCu.TrangThai != TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
            {
                TempData["Loi"] = "Phân công này không còn hiệu lực, không thể thay đổi.";
                return RedirectToAction(nameof(Index));
            }

            if (phanCongCu.MaGiangVien == maGiangVien)
            {
                TempData["Loi"] = "Giảng viên mới phải khác giảng viên đang hướng dẫn.";
                return RedirectToAction(nameof(ThayDoiGiangVien), new { maPhanCong });
            }

            // Giảng viên cũ đã kết thúc tải nên chỉ cần kiểm tra tải của giảng viên mới
            int taiGvMoi = await _nghipVu.TaiHienTaiGiangVienAsync(maGiangVien);
            var gvMoi = await _context.GiangViens.AsNoTracking()
                .FirstOrDefaultAsync(g => g.MaGiangVien == maGiangVien);
            if (gvMoi == null)
            {
                TempData["Loi"] = "Giảng viên không tồn tại.";
                return RedirectToAction(nameof(ThayDoiGiangVien), new { maPhanCong });
            }
            if (gvMoi.TrangThai != TrangThaiHeThong.GiangVien.DangCongTac)
            {
                TempData["Loi"] = $"Giảng viên {gvMoi.HoTen} đang tạm ngừng hướng dẫn.";
                return RedirectToAction(nameof(ThayDoiGiangVien), new { maPhanCong });
            }
            if (taiGvMoi >= gvMoi.SoSinhVienHuongDanToiDa)
            {
                TempData["Loi"] = $"Giảng viên {gvMoi.HoTen} đã đạt giới hạn {gvMoi.SoSinhVienHuongDanToiDa} sinh viên hướng dẫn.";
                return RedirectToAction(nameof(ThayDoiGiangVien), new { maPhanCong });
            }

            var maLinhVucDeTai = phanCongCu.DangKyDeTai?.DeTai?.MaLinhVuc;
            if (maLinhVucDeTai != null && gvMoi.MaLinhVuc != maLinhVucDeTai)
            {
                TempData["Loi"] = "Lĩnh vực của giảng viên không phù hợp với lĩnh vực của đề tài.";
                return RedirectToAction(nameof(ThayDoiGiangVien), new { maPhanCong });
            }

            // 1. Đóng phân công cũ, giữ nguyên để lưu lịch sử (mục 8.5)
            phanCongCu.TrangThai = TrangThaiHeThong.PhanCongHuongDan.DaThayDoi;
            phanCongCu.NgayKetThuc = ngayBatDau.Date.AddDays(-1);

            // 2. Tạo phân công mới bắt đầu từ ngày thay đổi
            var phanCongMoi = new PhanCongHuongDan
            {
                MaDangKy = phanCongCu.MaDangKy,
                MaGiangVien = maGiangVien,
                NgayBatDau = ngayBatDau.Date,
                NgayKetThuc = null,
                TrangThai = TrangThaiHeThong.PhanCongHuongDan.DangHuongDan,
                GhiChu = string.IsNullOrWhiteSpace(ghiChu)
                    ? $"Thay đổi từ giảng viên {phanCongCu.MaGiangVien}."
                    : ghiChu.Trim()
            };

            _context.PhanCongHuongDans.Add(phanCongMoi);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = $"Đã đổi giảng viên hướng dẫn sang {gvMoi.HoTen}.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Kết thúc hướng dẫn (mục 8.5)
        // Kết thúc thì đăng ký trở về trạng thái cần được phân công lại nếu chưa hoàn thành.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KetThuc(int maPhanCong, string? lyDo)
        {
            var phanCong = await _context.PhanCongHuongDans
                .FirstOrDefaultAsync(p => p.MaPhanCong == maPhanCong);

            if (phanCong == null)
            {
                return NotFound();
            }

            if (phanCong.TrangThai != TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
            {
                TempData["Loi"] = "Phân công này đã kết thục trước đó.";
                return RedirectToAction(nameof(Index));
            }

            phanCong.TrangThai = TrangThaiHeThong.PhanCongHuongDan.KetThuc;
            phanCong.NgayKetThuc = DateTime.Now;
            phanCong.GhiChu = string.IsNullOrWhiteSpace(lyDo) ? "Kết thúc hướng dẫn." : lyDo.Trim();
            await _context.SaveChangesAsync();

            // Kết thúc hướng dẫn nhưng đề tài chưa xong thì sinh viên cần được phân công lại.
            // Mục 8.6 chỉ có 3 trạng thái thực hiện, nên đưa về "Chưa bắt đầu";
            // việc "cần phân công lại" suy ra từ việc không còn phân công Đang hướng dẫn.
            var dangKy = await _context.DangKyDeTais.FirstOrDefaultAsync(d => d.MaDangKy == phanCong.MaDangKy);
            if (dangKy != null
                && dangKy.TrangThaiThucHien != TrangThaiHeThong.TrangThaiThucHien.HoanThanh)
            {
                dangKy.TrangThaiThucHien = TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau;
                await _context.SaveChangesAsync();
            }

            TempData["ThongBao"] = "Đã kết thúc phân công hướng dẫn.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Lịch sử phân công của một sinh viên (mục 8.5)
        public async Task<IActionResult> LichSu(string? maSinhVien)
        {
            if (string.IsNullOrEmpty(maSinhVien))
            {
                return NotFound();
            }

            ViewData["MaSinhVien"] = maSinhVien;
            var ds = await _context.PhanCongHuongDans
                .AsNoTracking()
                .Include(p => p.GiangVien)
                .Include(p => p.DangKyDeTai).ThenInclude(d => d!.DeTai)
                .Where(p => p.DangKyDeTai.MaSinhVien == maSinhVien)
                .OrderByDescending(p => p.NgayBatDau)
                .ToListAsync();

            return View(ds);
        }

        /// <summary>
        /// Nạp danh sách giảng viên hợp lệ cho một đề tài: đúng lĩnh vực, đang hoạt động, còn dưới giới hạn tải.
        /// Trả về cả giảng viên đang hướng dẫn (loạiTrang) để dùng khi sửa thì cho chọn lại chính người đó.
        /// </summary>
        private async Task NapDanhSachGiangVienAsync(int maDeTai, string? loaiTrang)
        {
            var maLinhVuc = await _context.DeTais.AsNoTracking()
                .Where(d => d.MaDeTai == maDeTai)
                .Select(d => d.MaLinhVuc)
                .FirstOrDefaultAsync();

            var taiHienTai = await _nghipVu.TaiHienTaiNhieuGiangVienAsync();

            var ds = await _context.GiangViens.AsNoTracking()
                .Where(g => g.MaLinhVuc == maLinhVuc
                            && g.TrangThai == TrangThaiHeThong.GiangVien.DangCongTac)
                .ToListAsync();

            // Mục 8.4: loại giảng viên đã đạt giới hạn khỏi danh sách chọn
            ds.RemoveAll(g => taiHienTai.TryGetValue(g.MaGiangVien, out var s)
                              && s >= g.SoSinhVienHuongDanToiDa);

            ViewData["DanhSachGv"] = ds;
            ViewData["TaiHienTai"] = taiHienTai;
            ViewData["MaGiangVienHienTai"] = loaiTrang;
        }
    }
}