// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Lớp nghiệp vụ dùng chung cho Module 3 và Module 4 (đăng ký - duyệt - phân công GVHD)
// Theo mục 12.1 đề tài: quy tắc phụ thuộc Database hoặc nhiều Entity phải kiểm tra bằng LINQ tại lớp nghiệp vụ.
//
// Vì sao cần lớp riêng?
// - Mục 7.6: "Mọi kiểm tra số chỗ phải thực hiện lại tại thời điểm duyệt, không chỉ khi sinh viên mở trang đăng ký."
//   Nên cùng một quy tắc (đề tài còn chỗ, giảng viên còn tải) phải chạy ở cả Module 3 lúc sinh viên đăng ký
//   và Module 4 lúc Admin duyệt / phân công. Gom vào 1 chỗ để hai bên không lệch logic.
// - Mục 25: SoChoCon và TaiHienTai là hai giá trị KHÔNG được nhập tay, luôn tính từ dữ liệu thật.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services
{
    /// <summary>Kết quả của một lần kiểm tra nghiệp vụ: hợp lệ thì không có thông báo lỗi.</summary>
    public class KiemTraKetQua
    {
        public bool HopLe { get; private init; }
        public string? ThongBao { get; private init; }

        /// <summary>Tên trường cần gắn lỗi. Null nghĩa là lỗi chung của cả form.</summary>
        public string? Truong { get; private init; }

        public static KiemTraKetQua OK() => new() { HopLe = true };

        public static KiemTraKetQua Sai(string thongBao, string? truong = null)
            => new() { HopLe = false, ThongBao = thongBao, Truong = truong };

        /// <summary>Chuyển lỗi sang ModelState để View hiển thị đúng chỗ.</summary>
        public void GanVaoModelState(ModelStateDictionary modelState)
        {
            if (!HopLe)
            {
                modelState.AddModelError(Truong ?? string.Empty, ThongBao ?? string.Empty);
            }
        }
    }

    /// <summary>
    /// Tập trung toàn bộ truy vấn LINQ tính toán và kiểm tra nghiệp vụ của phần đăng ký / duyệt / phân công.
    /// </summary>
    public class NghipVuService
    {
        private readonly AppDbContext _context;

        public NghipVuService(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================================
        // PHẦN 1: TÍNH TOÁN SỐ CHỖ CÒN VÀ TẢI HƯỚNG DẪN (mục 6.7 và 8.4)
        // =====================================================================

        /// <summary>
        /// Số sinh viên đang chiếm chỗ trên đề tài (mục 6.7).
        /// Chỉ tính các đăng ký có trạng thái hợp lệ (Đã duyệt). Từ chối / Đã hủy không tính.
        /// </summary>
        public Task<int> SoSinhVienDangKyHopLeAsync(int maDeTai)
        {
            return _context.DangKyDeTais
                .AsNoTracking()
                .CountAsync(d => d.MaDeTai == maDeTai && d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet);
        }

        /// <summary>
        /// SoChoCon = SoSinhVienToiDa - SoSinhVienDangKyHopLe (mục 25).
        /// Giá trị này luôn được tính lại từ Database, không bao giờ lưu vào DeTai.
        /// </summary>
        public async Task<int> TinhSoChoConAsync(int maDeTai)
        {
            var deTai = await _context.DeTais.AsNoTracking()
                .Where(d => d.MaDeTai == maDeTai)
                .Select(d => new { d.SoSinhVienToiDa })
                .FirstOrDefaultAsync();

            if (deTai == null)
            {
                return 0;
            }

            return deTai.SoSinhVienToiDa - await SoSinhVienDangKyHopLeAsync(maDeTai);
        }

        /// <summary>Bản đồ MaDeTai -> SoChoCon, phục vụ lọc "Còn chỗ / Hết chỗ" và sắp xếp (mục 6.8).</summary>
        public async Task<Dictionary<int, int>> TinhSoChoConNhieuAsync(IEnumerable<int> danhSachMaDeTai)
        {
            var ds = danhSachMaDeTai.Distinct().ToList();
            if (ds.Count == 0)
            {
                return new Dictionary<int, int>();
            }

            var soDaDuyet = await _context.DangKyDeTais.AsNoTracking()
                .Where(d => ds.Contains(d.MaDeTai) && d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet)
                .GroupBy(d => d.MaDeTai)
                .Select(g => new { MaDeTai = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.MaDeTai, x => x.SoLuong);

            return _context.DeTais.AsNoTracking()
                .Where(d => ds.Contains(d.MaDeTai))
                .Select(d => new { d.MaDeTai, d.SoSinhVienToiDa })
                .ToList() // Client-side để dùng kết quả nhóm ở trên
                .ToDictionary(
                    x => x.MaDeTai,
                    x => x.SoSinhVienToiDa - (soDaDuyet.TryGetValue(x.MaDeTai, out var s) ? s : 0));
        }

        /// <summary>
        /// Tải hướng dẫn hiện tại của giảng viên (mục 8.4) = số phân công có trạng thái Đang hướng dẫn.
        /// Phân công Đã thay đổi / Kết thúc không tính vào tải hiện tại.
        /// </summary>
        public Task<int> TaiHienTaiGiangVienAsync(string maGiangVien)
        {
            return _context.PhanCongHuongDans
                .AsNoTracking()
                .CountAsync(p => p.MaGiangVien == maGiangVien && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan);
        }

        /// <summary>Bản đồ MaGiangVien -> tải hiện tại, phục vụ màn hình phân công và thống kê (mục 8.8).</summary>
        public Task<Dictionary<string, int>> TaiHienTaiNhieuGiangVienAsync()
        {
            return _context.PhanCongHuongDans
                .AsNoTracking()
                .Where(p => p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
                .GroupBy(p => p.MaGiangVien)
                .Select(g => new { MaGiangVien = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.MaGiangVien, x => x.SoLuong);
        }

        // =====================================================================
        // PHẦN 2: ĐIỀU KIỆN ĐĂNG KÝ ĐỀ TÀI (mục 7.3)
        // =====================================================================

        /// <summary>
        /// Kiểm tra toàn bộ điều kiện trước khi sinh viên gửi đăng ký (mục 7.3).
        /// Trả về nguyên nhân cụ thể để hiển thị cho sinh viên.
        /// </summary>
        public async Task<KiemTraKetQua> KiemTraDieuKienDangKyAsync(string maSinhVien, int maDeTai)
        {
            // 1. Sinh viên tồn tại và đang hoạt động
            bool svHopLe = await _context.SinhViens.AsNoTracking()
                .AnyAsync(s => s.MaSinhVien == maSinhVien && s.TrangThai == TrangThaiHeThong.SinhVien.HoatDong);
            if (!svHopLe)
            {
                return KiemTraKetQua.Sai("Tài khoản sinh viên không tồn tại hoặc đã bị khóa.");
            }

            // 2. Đề tài tồn tại và đang Mở đăng ký
            var deTai = await _context.DeTais.AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDeTai == maDeTai && d.TrangThai == TrangThaiHeThong.DeTai.MoDangKy);
            if (deTai == null)
            {
                return KiemTraKetQua.Sai("Đề tài không tồn tại hoặc đã đóng đăng ký.", nameof(DangKyDeTai.MaDeTai));
            }

            // 3. Đề tài còn khả năng tiếp nhận sinh viên (số chỗ còn được tính lại tại thời điểm này)
            if (await TinhSoChoConAsync(maDeTai) <= 0)
            {
                return KiemTraKetQua.Sai("Rất tiếc, đề tài này đã hết chỗ.", nameof(DangKyDeTai.MaDeTai));
            }

            // 4. Sinh viên không có đăng ký Đã duyệt nào khác trong cùng đợt
            bool daCoDeTaiHieuLuc = await _context.DangKyDeTais.AsNoTracking()
                .AnyAsync(d => d.MaSinhVien == maSinhVien && d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet);
            if (daCoDeTaiHieuLuc)
            {
                return KiemTraKetQua.Sai("Bạn đã có đề tài được duyệt, không thể đăng ký thêm.");
            }

            // 5. Không tạo nhiều đăng ký Chờ duyệt cho cùng một đề tài
            bool daChoDuyet = await _context.DangKyDeTais.AsNoTracking()
                .AnyAsync(d => d.MaSinhVien == maSinhVien
                               && d.MaDeTai == maDeTai
                               && d.TrangThai == TrangThaiHeThong.DangKyDeTai.ChoDuyet);
            if (daChoDuyet)
            {
                return KiemTraKetQua.Sai("Bạn đã gửi yêu cầu cho đề tài này và đang chờ duyệt rồi.");
            }

            return KiemTraKetQua.OK();
        }

        // =====================================================================
        // PHẦN 3: ĐIỀU KIỆN DUYỆT ĐĂNG KÝ (mục 8.1)
        // =====================================================================

        /// <summary>
        /// Admin phải kiểm tra lại trước khi duyệt (mục 8.1 và 7.6).
        /// Không được tin vào kết quả kiểm tra lúc sinh viên đăng ký, vì số chỗ có thể đã đầy.
        /// </summary>
        public async Task<KiemTraKetQua> KiemTraDieuKienDuyetAsync(DangKyDeTai dangKy)
        {
            // Đăng ký chưa bị hủy hoặc xử lý trước đó
            if (dangKy.TrangThai != TrangThaiHeThong.DangKyDeTai.ChoDuyet)
            {
                return KiemTraKetQua.Sai("Đăng ký này đã được xử lý trước đó, không thể duyệt lại.");
            }

            // Sinh viên vẫn đang hoạt động
            bool svHopLe = await _context.SinhViens.AsNoTracking()
                .AnyAsync(s => s.MaSinhVien == dangKy.MaSinhVien && s.TrangThai == TrangThaiHeThong.SinhVien.HoatDong);
            if (!svHopLe)
            {
                return KiemTraKetQua.Sai("Sinh viên không còn hoạt động, không thể duyệt đăng ký.");
            }

            // Sinh viên chưa có đề tài khác đang hiệu lực (loại trừ chính đăng ký đang xét)
            bool daCoDeTaiKhac = await _context.DangKyDeTais.AsNoTracking()
                .AnyAsync(d => d.MaSinhVien == dangKy.MaSinhVien
                               && d.MaDangKy != dangKy.MaDangKy
                               && d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet);
            if (daCoDeTaiKhac)
            {
                return KiemTraKetQua.Sai("Sinh viên đã có đề tài khác được duyệt.");
            }

            // Đề tài vẫn còn chỗ (tính lại tại thời điểm duyệt)
            if (await TinhSoChoConAsync(dangKy.MaDeTai) <= 0)
            {
                return KiemTraKetQua.Sai("Đề tài đã đủ số lượng, không thể duyệt thêm.");
            }

            return KiemTraKetQua.OK();
        }

        // =====================================================================
        // PHẦN 4: ĐIỀU KIỆN PHÂN CÔNG / THAY ĐỔI GIẢNG VIÊN (mục 8.3 và 8.4)
        // =====================================================================

        /// <summary>
        /// Kiểm tra trước khi lưu một phân công (mục 8.3).
        /// Dùng chung cho cả lần phân công đầu tiên và lần thay đổi giảng viên (mục 8.5).
        /// </summary>
        public async Task<KiemTraKetQua> KiemTraDieuKienPhanCongAsync(int maDangKy, string maGiangVien)
        {
            var dangKy = await _context.DangKyDeTais.AsNoTracking()
                .Include(d => d.DeTai)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            // Chỉ đăng ký Đã duyệt mới được phân công
            if (dangKy == null)
            {
                return KiemTraKetQua.Sai("Không tìm thấy đăng ký đề tài.");
            }
            if (dangKy.TrangThai != TrangThaiHeThong.DangKyDeTai.DaDuyet)
            {
                return KiemTraKetQua.Sai("Chỉ đăng ký Đã duyệt mới được phân công giảng viên hướng dẫn.");
            }

            // Sinh viên và đề tài vẫn ở trạng thái hợp lệ
            bool svHopLe = await _context.SinhViens.AsNoTracking()
                .AnyAsync(s => s.MaSinhVien == dangKy.MaSinhVien && s.TrangThai == TrangThaiHeThong.SinhVien.HoatDong);
            if (!svHopLe)
            {
                return KiemTraKetQua.Sai("Sinh viên không còn hoạt động.");
            }
            if (dangKy.DeTai == null || dangKy.DeTai.TrangThai == TrangThaiHeThong.DeTai.KetThuc)
            {
                return KiemTraKetQua.Sai("Đề tài đã kết thúc, không thể phân công giảng viên hướng dẫn.");
            }

            // Giảng viên tồn tại và đang được phép hướng dẫn
            var giangVien = await _context.GiangViens.AsNoTracking()
                .FirstOrDefaultAsync(g => g.MaGiangVien == maGiangVien);
            if (giangVien == null)
            {
                return KiemTraKetQua.Sai("Giảng viên không tồn tại.");
            }
            if (giangVien.TrangThai != TrangThaiHeThong.GiangVien.DangCongTac)
            {
                return KiemTraKetQua.Sai($"Giảng viên {giangVien.HoTen} đang tạm ngừng hướng dẫn, không thể phân công mới.");
            }

            // Lĩnh vực của giảng viên phù hợp với lĩnh vực của đề tài
            if (dangKy.DeTai != null && giangVien.MaLinhVuc != dangKy.DeTai.MaLinhVuc)
            {
                return KiemTraKetQua.Sai("Lĩnh vực của giảng viên không phù hợp với lĩnh vực của đề tài.");
            }

            // Giảng viên chưa vượt SoSinhVienHuongDanToiDa
            int taiHienTai = await TaiHienTaiGiangVienAsync(maGiangVien);
            if (taiHienTai >= giangVien.SoSinhVienHuongDanToiDa)
            {
                return KiemTraKetQua.Sai($"Giảng viên {giangVien.HoTen} đã đạt giới hạn {giangVien.SoSinhVienHuongDanToiDa} sinh viên hướng dẫn.");
            }

            // Đăng ký chưa có một phân công khác ở trạng thái Đang hướng dẫn
            bool daCoPhanCong = await _context.PhanCongHuongDans.AsNoTracking()
                .AnyAsync(p => p.MaDangKy == maDangKy && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan);
            if (daCoPhanCong)
            {
                return KiemTraKetQua.Sai("Đăng ký này đã có giảng viên đang hướng dẫn. Hãy dùng chức năng Thay đổi giảng viên.");
            }

            return KiemTraKetQua.OK();
        }

        /// <summary>
        /// Tải hiện tại của giảng viên sau khi đã loại trừ một phân công sắp kết thúc (dùng cho lúc thay đổi GVHD).
        /// </summary>
        public async Task<int> TaiGiangVienTruocKhiKetThucAsync(int maPhanCong)
        {
            var phanCong = await _context.PhanCongHuongDans.AsNoTracking()
                .FirstOrDefaultAsync(p => p.MaPhanCong == maPhanCong);
            if (phanCong == null || phanCong.TrangThai != TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
            {
                return 0;
            }

            return await TaiHienTaiGiangVienAsync(phanCong.MaGiangVien) - 1;
        }

        /// <summary>Giảng viên hướng dẫn hiện tại của một đăng ký, hoặc null nếu chưa được phân công (mục 7.5).</summary>
        public Task<GiangVien?> GiangVienHienTaiCuaDangKyAsync(int maDangKy)
        {
            return _context.PhanCongHuongDans
                .AsNoTracking()
                .Include(p => p.GiangVien)
                .Where(p => p.MaDangKy == maDangKy && p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
                .Select(p => p.GiangVien)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cập nhật trạng thái đề tài theo số chỗ còn thực tế (Mở đăng ký <-> Đủ số lượng).
        /// Tránh phải Admin tự chỉnh tay, nhờ vậy số liệu Dashboard luôn khớp dữ liệu.
        /// </summary>
        public async Task DongBoTrangThaiDeTaiAsync(int maDeTai)
        {
            var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.MaDeTai == maDeTai);

            // Đề tài đã Tạm khóa / Kết thúc thì giữ nguyên trạng thái do Admin quyết định
            if (deTai == null || deTai.TrangThai is TrangThaiHeThong.DeTai.TamKhoa or TrangThaiHeThong.DeTai.KetThuc)
            {
                return;
            }

            int soChoCon = deTai.SoSinhVienToiDa - await SoSinhVienDangKyHopLeAsync(maDeTai);
            string trangThaiMoi = soChoCon > 0
                ? TrangThaiHeThong.DeTai.MoDangKy
                : TrangThaiHeThong.DeTai.DuSoLuong;

            if (deTai.TrangThai != trangThaiMoi)
            {
                deTai.TrangThai = trangThaiMoi;
                await _context.SaveChangesAsync();
            }
        }
    }
}