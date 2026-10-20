// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 3 - Giao diện Đăng ký đề tài, kiểm tra điều kiện bằng LINQ

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class DangKyDeTaisController : Controller
    {
        // TẠM THỜI: giả lập sinh viên đang đăng nhập.
        // Khi ghép với Module 1, thay hằng số này bằng Session
        // (VD: HttpContext.Session.GetString("MaSinhVien"))
        private const string MaSinhVienHienTai = "SV001";

        private readonly AppDbContext _context;
        private readonly NghipVuService _nghipVu;

        public DangKyDeTaisController(AppDbContext context, NghipVuService nghipVu)
        {
            _context = context;
            _nghipVu = nghipVu;
        }

        // GET: Mở form đăng ký
        public async Task<IActionResult> Create()
        {
            // Chỉ load các đề tài đang ở trạng thái "MoDangKy" ra cho sinh viên chọn
            await NapDanhSachDeTaiMoDangKyAsync();
            return View(new DangKyDeTaiCreateViewModel());
        }

        // POST: Xử lý nghiệp vụ lưu đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DangKyDeTaiCreateViewModel input)
        {
            // 0. Chặn ngay nếu dữ liệu form không hợp lệ (chưa chọn đề tài, lý do quá dài...)
            if (!ModelState.IsValid)
            {
                await NapDanhSachDeTaiMoDangKyAsync(input.MaDeTai);
                return View(input);
            }

            // ==========================================
            // KIỂM TRA NGHIỆP VỤ BẰNG LINQ
            // ==========================================
            // Toàn bộ quy tắc mục 7.3 nằm trong NghipVuService để dùng chung với Module 4.
            // Ở đây chỉ gọi lại và đưa lỗi vào ModelState, không viết lại logic.
            var kiemTra = await _nghipVu.KiemTraDieuKienDangKyAsync(MaSinhVienHienTai, input.MaDeTai!.Value);
            kiemTra.GanVaoModelState(ModelState);

            // Nếu có lỗi thì nạp lại danh sách đề tài và trả View để hiển thị lỗi đỏ
            if (!ModelState.IsValid)
            {
                await NapDanhSachDeTaiMoDangKyAsync(input.MaDeTai);
                return View(input);
            }

            // Không có lỗi: tạo bản ghi. Các trường do hệ thống quyết định được gán ở đây
            var dangKyDeTai = new DangKyDeTai
            {
                MaSinhVien = MaSinhVienHienTai,
                MaDeTai = input.MaDeTai.Value,
                NgayDangKy = DateTime.Now,
                LyDoDangKy = input.LyDoDangKy,
                TrangThai = TrangThaiHeThong.DangKyDeTai.ChoDuyet,
                TrangThaiThucHien = TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau
            };

            _context.Add(dangKyDeTai);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đã gửi yêu cầu đăng ký đề tài thành công. Vui lòng chờ giảng viên duyệt.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Xem lịch sử đăng ký đề tài của sinh viên
        public async Task<IActionResult> Index()
        {
            // Truy vấn LINQ: lấy danh sách đăng ký của sinh viên hiện tại, kết bảng để lấy thông tin Đề tài
            var danhSachDangKy = await _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.DeTai)
                .Where(d => d.MaSinhVien == MaSinhVienHienTai)
                .OrderByDescending(d => d.NgayDangKy) // Yêu cầu mới nhất lên đầu
                .ToListAsync();

            // Mục 7.5: sinh viên phải thấy được giảng viên hướng dẫn hiện tại của từng đăng ký
            var maDangKys = danhSachDangKy.Select(d => d.MaDangKy).ToList();
            var gvHienTai = await _context.PhanCongHuongDans
                .AsNoTracking()
                .Include(p => p.GiangVien)
                .Where(p => maDangKys.Contains(p.MaDangKy)
                            && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
                .ToDictionaryAsync(p => p.MaDangKy, p => p.GiangVien!.HoTen);

            ViewData["GvHienTai"] = gvHienTai;
            return View(danhSachDangKy);
        }

        // GET: Chi tiết một đăng ký của chính sinh viên (mục 7.5)
        public async Task<IActionResult> Details(int? maDangKy)
        {
            if (maDangKy == null)
            {
                return NotFound();
            }

            var dangKy = await _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.DeTai).ThenInclude(t => t.LinhVuc)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy && d.MaSinhVien == MaSinhVienHienTai);

            // Mục 7.5: sinh viên không được xem đăng ký của sinh viên khác bằng cách đổi id trên URL
            if (dangKy == null)
            {
                return NotFound();
            }

            // Lịch sử phân công giảng viên liên quan (mục 7.5 và 8.5)
            ViewData["LichSuPhanCong"] = await _context.PhanCongHuongDans
                .AsNoTracking()
                .Include(p => p.GiangVien)
                .Where(p => p.MaDangKy == maDangKy)
                .OrderByDescending(p => p.NgayBatDau)
                .ToListAsync();

            return View(dangKy);
        }

        // GET: Mở xác nhận hủy đăng ký
        public async Task<IActionResult> HuyDangKy(int? maDangKy)
        {
            if (maDangKy == null)
            {
                return NotFound();
            }

            var dangKy = await _context.DangKyDeTais
                .AsNoTracking()
                .Include(d => d.DeTai)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy && d.MaSinhVien == MaSinhVienHienTai);

            if (dangKy == null)
            {
                return NotFound();
            }

            return View(dangKy);
        }

        // POST: Hủy đăng ký (mục 7.4)
        // Chỉ được hủy khi đăng ký còn ở trạng thái Chờ duyệt.
        // Đăng ký Đã duyệt không được tự ý xóa để tránh mất lịch sử.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int maDangKy)
        {
            var dangKy = await _context.DangKyDeTais
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy && d.MaSinhVien == MaSinhVienHienTai);

            if (dangKy == null)
            {
                return NotFound();
            }

            if (dangKy.TrangThai != TrangThaiHeThong.DangKyDeTai.ChoDuyet)
            {
                TempData["Loi"] = "Chỉ được hủy đăng ký đang ở trạng thái Chờ duyệt. "
                                + "Đăng ký đã được duyệt phải liên hệ Admin để thay đổi.";
                return RedirectToAction(nameof(Index));
            }

            dangKy.TrangThai = TrangThaiHeThong.DangKyDeTai.DaHuy;
            dangKy.GhiChuDuyet = "Sinh viên tự hủy đăng ký.";
            await _context.SaveChangesAsync();

            // Số chỗ còn không bị ảnh hưởng vì đăng ký Hủy chưa từng chiếm chỗ,
            // nhưng vẫn đồng bộ trạng thái đề tài cho chắc chắn.
            await _nghipVu.DongBoTrangThaiDeTaiAsync(dangKy.MaDeTai);

            TempData["ThongBao"] = "Đã hủy đăng ký đề tài.";
            return RedirectToAction(nameof(Index));
        }

        // Nạp danh sách đề tài đang mở đăng ký vào ViewBag để hiển thị ở dropdown
        private async Task NapDanhSachDeTaiMoDangKyAsync(object? maDeTaiDaChon = null)
        {
            var deTais = await _context.DeTais
                .AsNoTracking()
                .Where(d => d.TrangThai == "MoDangKy")
                .OrderBy(d => d.TenDeTai)
                .ToListAsync();

            ViewData["MaDeTai"] = new SelectList(deTais, nameof(DeTai.MaDeTai), nameof(DeTai.TenDeTai), maDeTaiDaChon);
        }
    }
}