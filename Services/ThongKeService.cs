// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Lớp truy vấn LINQ cho Dashboard và màn hình Thống kê (Module 4)
// Mục 8.8: "Các số liệu phải lấy trực tiếp từ Database bằng LINQ tại thời điểm truy vấn;
//             không nhập hoặc lưu các tổng số cố định."

using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services
{
    public class ThongKeService
    {
        private readonly AppDbContext _context;
        private readonly NghipVuService _nghipVu;

        public ThongKeService(AppDbContext context, NghipVuService nghipVu)
        {
            _context = context;
            _nghipVu = nghipVu;
        }

        /// <summary>Lấy toàn bộ số liệu Dashboard (mục 8.7).</summary>
        public async Task<DashboardSoLieu> LaySoLieuDashboardAsync()
        {
            var soLieu = new DashboardSoLieu
            {
                TongLinhVuc = await _context.LinhVucs.AsNoTracking().CountAsync(),
                TongGiangVien = await _context.GiangViens.AsNoTracking().CountAsync(),
                TongSinhVien = await _context.SinhViens.AsNoTracking().CountAsync(),
                TongDeTai = await _context.DeTais.AsNoTracking().CountAsync(),

                SoDangKyChoDuyet = await _context.DangKyDeTais.AsNoTracking()
                    .CountAsync(d => d.TrangThai == TrangThaiHeThong.DangKyDeTai.ChoDuyet),
                SoDangKyDaDuyet = await _context.DangKyDeTais.AsNoTracking()
                    .CountAsync(d => d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet),

                // Số sinh viên đang có giảng viên hướng dẫn = số phân công Đang hướng dẫn
                SoSinhVienDangCoGiangVien = await _context.PhanCongHuongDans.AsNoTracking()
                    .CountAsync(p => p.TrangThai == TrangThaiHeThong.PhanCongHuongDan.DangHuongDan)
            };

            // Số đề tài còn chỗ / đủ số lượng phải tính từ số đăng ký thật, không đọc thẳng trường TrangThai
            var soChoCon = await _nghipVu.TinhSoChoConNhieuAsync(
                await _context.DeTais.AsNoTracking().Select(d => d.MaDeTai).ToListAsync());

            soLieu.SoDeTaiConCho = soChoCon.Count(x => x.Value > 0);
            soLieu.SoDeTaiDuSoLuong = soChoCon.Count(x => x.Value <= 0);

            // Giảng viên đã đạt giới hạn hướng dẫn
            var taiHienTai = await _nghipVu.TaiHienTaiNhieuGiangVienAsync();
            var gioiHan = await _context.GiangViens.AsNoTracking()
                .ToDictionaryAsync(g => g.MaGiangVien, g => g.SoSinhVienHuongDanToiDa);

            soLieu.SoGiangVienDatGioiHan = taiHienTai.Count(
                t => gioiHan.TryGetValue(t.Key, out var limit) && t.Value >= limit);

            return soLieu;
        }

        /// <summary>Số đề tài theo từng lĩnh vực (mục 8.8) - dùng GroupBy.</summary>
        public async Task<List<ThongKeDeTaiTheoLinhVuc>> ThongKeDeTaiTheoLinhVucAsync()
        {
            // GroupBy chạy trong SQL, sau đó ghép tên lĩnh vực ở phía C#
            var theoMaLinhVuc = await _context.DeTais.AsNoTracking()
                .GroupBy(d => d.MaLinhVuc)
                .Select(g => new { MaLinhVuc = g.Key, SoDeTai = g.Count() })
                .ToListAsync();

            var tenLinhVuc = await _context.LinhVucs.AsNoTracking()
                .ToDictionaryAsync(l => l.MaLinhVuc, l => l.TenLinhVuc);

            return theoMaLinhVuc
                .Select(x => new ThongKeDeTaiTheoLinhVuc
                {
                    TenLinhVuc = tenLinhVuc.TryGetValue(x.MaLinhVuc, out var t) ? t : "-",
                    SoDeTai = x.SoDeTai
                })
                .OrderByDescending(x => x.SoDeTai)
                .ToList();
        }

        /// <summary>Số đăng ký theo trạng thái (mục 8.8) - dùng GroupBy.</summary>
        public async Task<List<ThongKeDangKyTheoTrangThai>> ThongKeDangKyTheoTrangThaiAsync()
        {
            return await _context.DangKyDeTais.AsNoTracking()
                .GroupBy(d => d.TrangThai)
                .Select(g => new ThongKeDangKyTheoTrangThai
                {
                    TrangThai = g.Key,
                    SoLuong = g.Count()
                })
                .ToListAsync();
        }

        /// <summary>Số sinh viên đăng ký hợp lệ và số chỗ còn của từng đề tài (mục 8.8).</summary>
        public async Task<List<ThongKeDeTaiChiTiet>> ThongKeSoChoConTungDeTaiAsync()
        {
            var soDaDuyet = await _context.DangKyDeTais.AsNoTracking()
                .Where(d => d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet)
                .GroupBy(d => d.MaDeTai)
                .Select(g => new { MaDeTai = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.MaDeTai, x => x.SoLuong);

            // Lưu ý: phải có ngoặc quanh await, vì "await a.ToListAsync().Select(...)"
            // sẽ được hiểu là await (Task.Select(...)) chứ không phải (await Task).Select(...)
            var dsDeTai = await _context.DeTais.AsNoTracking()
                .Include(d => d.LinhVuc)
                .OrderByDescending(d => d.NamHoc).ThenBy(d => d.HocKy)
                .ToListAsync();

            return dsDeTai // Client-side để dùng kết quả GroupBy ở trên
                .Select(d => new ThongKeDeTaiChiTiet
                {
                    MaDeTai = d.MaDeTai,
                    TenDeTai = d.TenDeTai,
                    TenLinhVuc = d.LinhVuc?.TenLinhVuc ?? "-",
                    NamHoc = d.NamHoc,
                    HocKy = d.HocKy,
                    SoSinhVienToiDa = d.SoSinhVienToiDa,
                    SoDangKyHopLe = soDaDuyet.TryGetValue(d.MaDeTai, out var s) ? s : 0,
                    TrangThai = d.TrangThai
                })
                .ToList();
        }

        /// <summary>
        /// Số sinh viên đang được từng giảng viên hướng dẫn và tỷ lệ sử dụng khả năng hướng dẫn (mục 8.8).
        /// </summary>
        public async Task<List<ThongKeTaiGiangVien>> ThongKeTaiGiangVienAsync()
        {
            var taiHienTai = await _nghipVu.TaiHienTaiNhieuGiangVienAsync();

            var dsGiangVien = await _context.GiangViens.AsNoTracking()
                .Include(g => g.LinhVuc)
                .ToListAsync();

            return dsGiangVien // Client-side để dùng kết quả GroupBy ở trên
                .Select(g => new ThongKeTaiGiangVien
                {
                    MaGiangVien = g.MaGiangVien,
                    HoTen = g.HoTen,
                    TenLinhVuc = g.LinhVuc?.TenLinhVuc ?? "-",
                    SoSinhVienHuongDanToiDa = g.SoSinhVienHuongDanToiDa,
                    SoDangHuongDanHienTai = taiHienTai.TryGetValue(g.MaGiangVien, out var s) ? s : 0,
                    TrangThai = g.TrangThai
                })
                .OrderByDescending(x => x.SoDangHuongDanHienTai)
                .ThenBy(x => x.HoTen)
                .ToList();
        }

        /// <summary>Giảng viên có số sinh viên hướng dẫn nhiều nhất (mục 8.8).</summary>
        public async Task<ThongKeTaiGiangVien?> TimGiangVienHuongDanNhieuNhatAsync()
        {
            var ds = await ThongKeTaiGiangVienAsync();
            return ds.FirstOrDefault();
        }

        /// <summary>Số đăng ký theo từng tháng (mục 8.8) - dùng GroupBy theo Year/Month.</summary>
        public async Task<List<ThongKeDangKyTheoThang>> ThongKeDangKyTheoThangAsync()
        {
            return await _context.DangKyDeTais.AsNoTracking()
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new ThongKeDangKyTheoThang
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoLuong = g.Count()
                })
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .ToListAsync();
        }

        /// <summary>Số đề tài theo năm học / học kỳ (mục 8.8).</summary>
        public async Task<List<ThongKeDeTaiTheoHocKy>> ThongKeDeTaiTheoHocKyAsync()
        {
            return await _context.DeTais.AsNoTracking()
                .GroupBy(d => new { d.NamHoc, d.HocKy })
                .Select(g => new ThongKeDeTaiTheoHocKy
                {
                    NamHoc = g.Key.NamHoc,
                    HocKy = g.Key.HocKy,
                    SoDeTai = g.Count()
                })
                .OrderByDescending(x => x.NamHoc).ThenBy(x => x.HocKy)
                .ToListAsync();
        }
    }
}