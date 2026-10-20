// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Nạp dữ liệu mẫu theo mục 15 đề tài
//   - 5 lĩnh vực, 12 giảng viên, 20 đề tài, 30 sinh viên (kèm tài khoản)
//   - 30 đăng ký đủ tình huống: Chờ duyệt, Đã duyệt, Từ chối, Đã hủy
//   - Lịch sử phân công GVHD có cả trường hợp "Đã thay đổi" và "Kết thúc"
//
// Dữ liệu được cố ý bảo đảm đúng luật nghiệp vụ (mục 7.3, 7.6, 8.3, 8.4) để số liệu Dashboard,
// Thống kê và các màn hình kiểm tra đều khớp với dữ liệu thật.

using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Thứ tự gọi quan trọng: phải tạo Lĩnh vực và Giảng viên trước,
            // vì DeTai và SinhVien tham chiếu tới những bảng này.
            if (!context.LinhVucs.Any())
            {
                context.LinhVucs.AddRange(
                    new LinhVuc { TenLinhVuc = "Công nghệ phần mềm", MoTa = "Phát triển ứng dụng web, mobile và hệ thống thông tin.", TrangThai = TrangThaiHeThong.LinhVuc.HoatDong },
                    new LinhVuc { TenLinhVuc = "Trí tuệ nhân tạo", MoTa = "Học máy, xử lý ngôn ngữ tự nhiên và thị giác máy tính.", TrangThai = TrangThaiHeThong.LinhVuc.HoatDong },
                    new LinhVuc { TenLinhVuc = "An toàn thông tin", MoTa = "Bảo mật hệ thống, phân tích malware và giám sát mạng.", TrangThai = TrangThaiHeThong.LinhVuc.HoatDong },
                    new LinhVuc { TenLinhVuc = "Phân tích dữ liệu lớn", MoTa = "Kho dữ liệu, khai phác và phân tích dữ liệu quy mô lớn.", TrangThai = TrangThaiHeThong.LinhVuc.HoatDong },
                    new LinhVuc { TenLinhVuc = "IoT và Hệ thống nhúng", MoTa = "Thiết bị kết nối và hệ thống nhúng thu thập dữ liệu.", TrangThai = TrangThaiHeThong.LinhVuc.HoatDong });
                context.SaveChanges();
            }

            if (!context.GiangViens.Any())
            {
                context.GiangViens.AddRange(
                    // Lĩnh vực 1 - Công nghệ phần mềm
                    new GiangVien { MaGiangVien = "GV001", HoTen = "TS. Nguyễn Văn An", HocVi = "Tiến sĩ", Email = "nguyenvanan@uneti.edu.vn", SoDienThoai = "0981000001", MaLinhVuc = 1, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV002", HoTen = "TS. Trần Thị Bình", HocVi = "Tiến sĩ", Email = "tranthibinh@uneti.edu.vn", SoDienThoai = "0981000002", MaLinhVuc = 1, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV011", HoTen = "ThS. Đỗ Văn Phúc", HocVi = "Thạc sĩ", Email = "dovanphuc@uneti.edu.vn", SoDienThoai = "0981000011", MaLinhVuc = 1, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.TamNgungHuongDan },

                    // Lĩnh vực 2 - Trí tuệ nhân tạo
                    new GiangVien { MaGiangVien = "GV003", HoTen = "TS. Lê Văn Chiến", HocVi = "Tiến sĩ", Email = "levanchien@uneti.edu.vn", SoDienThoai = "0981000003", MaLinhVuc = 2, SoSinhVienHuongDanToiDa = 2, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV004", HoTen = "TS. Phạm Thị Duyên", HocVi = "Tiến sĩ", Email = "phamthiduyen@uneti.edu.vn", SoDienThoai = "0981000004", MaLinhVuc = 2, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },

                    // Lĩnh vực 3 - An toàn thông tin
                    new GiangVien { MaGiangVien = "GV005", HoTen = "TS. Võ Văn Giang", HocVi = "Tiến sĩ", Email = "vovangian@uneti.edu.vn", SoDienThoai = "0981000005", MaLinhVuc = 3, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV006", HoTen = "ThS. Đặng Thị Hà", HocVi = "Thạc sĩ", Email = "dangthiha@uneti.edu.vn", SoDienThoai = "0981000006", MaLinhVuc = 3, SoSinhVienHuongDanToiDa = 2, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV012", HoTen = "ThS. Hoàng Thị Quỳnh", HocVi = "Thạc sĩ", Email = "hoangthiquynh@uneti.edu.vn", SoDienThoai = "0981000012", MaLinhVuc = 3, SoSinhVienHuongDanToiDa = 2, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },

                    // Lĩnh vực 4 - Phân tích dữ liệu lớn
                    new GiangVien { MaGiangVien = "GV007", HoTen = "TS. Bùi Văn Hùng", HocVi = "Tiến sĩ", Email = "buivanhung@uneti.edu.vn", SoDienThoai = "0981000007", MaLinhVuc = 4, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV008", HoTen = "TS. Ngô Thị Lan", HocVi = "Tiến sĩ", Email = "ngothilan@uneti.edu.vn", SoDienThoai = "0981000008", MaLinhVuc = 4, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },

                    // Lĩnh vực 5 - IoT và Hệ thống nhúng
                    new GiangVien { MaGiangVien = "GV009", HoTen = "TS. Trịnh Văn Minh", HocVi = "Tiến sĩ", Email = "trinhvanminh@uneti.edu.vn", SoDienThoai = "0981000009", MaLinhVuc = 5, SoSinhVienHuongDanToiDa = 2, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac },
                    new GiangVien { MaGiangVien = "GV010", HoTen = "ThS. Phan Thị Nga", HocVi = "Thạc sĩ", Email = "phanthinga@uneti.edu.vn", SoDienThoai = "0981000010", MaLinhVuc = 5, SoSinhVienHuongDanToiDa = 3, TrangThai = TrangThaiHeThong.GiangVien.DangCongTac });
                context.SaveChanges();
            }

            if (!context.TaiKhoans.Any())
            {
                // Mục 15: 30 tài khoản cho 30 sinh viên, mật khẩu thống nhất để tiện thử nghiệm
                const string matKhauChung = "123456";

                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "admin",
                    MatKhau = matKhauChung,
                    HoTen = "Quản trị viên",
                    Email = "admin@uneti.edu.vn",
                    VaiTro = TrangThaiHeThong.VaiTro.Admin,
                    TrangThai = TrangThaiHeThong.TaiKhoan.HoatDong
                });

                for (int i = 1; i <= 30; i++)
                {
                    context.TaiKhoans.Add(new TaiKhoan
                    {
                        TenDangNhap = $"sv{i:000}",
                        MatKhau = matKhauChung,
                        HoTen = $"Sinh viên {i:000}",
                        Email = $"sv{i:000}@uneti.edu.vn",
                        VaiTro = TrangThaiHeThong.VaiTro.SinhVien,
                        TrangThai = TrangThaiHeThong.TaiKhoan.HoatDong
                    });
                }
                context.SaveChanges();
            }

            if (!context.SinhViens.Any())
            {
                var dsTaiKhoan = context.TaiKhoans
                    .Where(t => t.VaiTro == TrangThaiHeThong.VaiTro.SinhVien)
                    .OrderBy(t => t.TenDangNhap)
                    .ToList();

                for (int i = 1; i <= 30; i++)
                {
                    var ma = $"SV{i:000}";
                    var tk = dsTaiKhoan.FirstOrDefault(t => t.TenDangNhap == $"sv{i:000}");
                    if (tk == null)
                    {
                        continue;
                    }

                    tk.HoTen = i == 1 ? "Đào Hoàng An" : $"Sinh viên {i:000}";

                    // SV001 là sinh viên đang đăng nhập (được hardcode trong DangKyDeTaisController),
                    // các sinh viên còn lại chia đều các trạng thái để có dữ liệu đủ tình huống.
                    string trangThai = i == 1
                        ? TrangThaiHeThong.SinhVien.HoatDong
                        : i == 29 ? TrangThaiHeThong.SinhVien.DinhChi
                        : i == 30 ? TrangThaiHeThong.SinhVien.DaTotNghiep
                        : TrangThaiHeThong.SinhVien.HoatDong;

                    context.SinhViens.Add(new SinhVien
                    {
                        MaSinhVien = ma,
                        MaTaiKhoan = tk.MaTaiKhoan,
                        HoTen = tk.HoTen,
                        NgaySinh = new DateTime(2003, 1, 1).AddDays(i * 37),
                        Lop = $"KTX{2024 + i % 3}K{(i % 4) + 1}",
                        Khoa = "Công nghệ thông tin",
                        ChuyenNganh = "Hệ thống thông tin",
                        Email = tk.Email,
                        SoDienThoai = $"0901{i:00000}",
                        TrangThai = trangThai
                    });
                }
                context.SaveChanges();
            }

            if (!context.DeTais.Any())
            {
                context.DeTais.AddRange(
                    new DeTai { TenDeTai = "Xây dựng website quản lý thư viện", MaLinhVuc = 1, MoTa = "Ứng dụng MVC quản lý sách, độc giả và mượn trả.", YeuCau = "C# ASP.NET Core MVC, SQL Server", SoSinhVienToiDa = 1, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Ứng dụng điểm danh bằng trí tuệ nhân tạo", MaLinhVuc = 2, MoTa = "Nhận diện khuôn mặt để điểm danh tự động.", YeuCau = "Python hoặc C# kết hợp thư viện AI", SoSinhVienToiDa = 2, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Website bán hàng trực tuyến", MaLinhVuc = 1, MoTa = "Cửa hàng điện tử kèm thanh toán và quản lý đơn hàng.", YeuCau = "ASP.NET Core MVC", SoSinhVienToiDa = 2, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Hệ thống quản lý công việc nhóm", MaLinhVuc = 1, MoTa = "Theo dõi nhiệm vụ, tiến độ và phân công thành viên.", YeuCau = "ASP.NET Core MVC", SoSinhVienToiDa = 2, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Nhận dạng giọng nói tiếng Việt", MaLinhVuc = 2, MoTa = "Chuyển giọng nói thành văn bản cho tiếng Việt.", YeuCau = "Xử lý ngôn ngữ tự nhiên", SoSinhVienToiDa = 1, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Phân tích dữ liệu mạng xã hội", MaLinhVuc = 4, MoTa = "Thu thập và phân tích dữ liệu người dùng mạng xã hội.", YeuCau = "SQL Server, công cụ trực quan hóa dữ liệu", SoSinhVienToiDa = 3, NamHoc = 2025, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Ứng dụng ngân hàng số", MaLinhVuc = 1, MoTa = "Mô phỏng giao dịch và quản lý tài khoản khách hàng.", YeuCau = "ASP.NET Core MVC", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Phát hiện gian lận học tập", MaLinhVuc = 2, MoTa = "Phát hiện hành vi sao chép trong bài kiểm tra.", YeuCau = "Học máy, phân tích văn bản", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Hệ thống giám sát an ninh mạng", MaLinhVuc = 3, MoTa = "Thu thập log và cảnh báo sự kiện bất thường.", YeuCau = "Linux, giám sát log, Power BI", SoSinhVienToiDa = 3, NamHoc = 2025, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Phân tích mã độc", MaLinhVuc = 3, MoTa = "Phân tích hành vi và đặc trưng của malware.", YeuCau = "Phân tích tĩnh và động mã độc", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Ứng dụng IoT nhà thông minh", MaLinhVuc = 5, MoTa = "Điều khiển thiết bị gia dụng từ xa.", YeuCau = "ESP32, MQTT, ASP.NET Core", SoSinhVienToiDa = 2, NamHoc = 2025, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Mạng cảm biến môi trường", MaLinhVuc = 5, MoTa = "Theo dõi nhiệt độ, độ ẩm và chất lượng không khí.", YeuCau = "Arduino, giao tiếp nối tiếp", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Nền tảng học trực tuyến", MaLinhVuc = 1, MoTa = "Quản lý khóa học, bài giảng và bài tập trực tuyến.", YeuCau = "ASP.NET Core MVC", SoSinhVienToiDa = 3, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Trợ lý ảo hỗ trợ học tập", MaLinhVuc = 2, MoTa = "Trợ lý ảo trả lời câu hỏi và gợi ý tài liệu.", YeuCau = "Xử lý ngôn ngữ tự nhiên", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Kho dữ liệu lớn cho doanh nghiệp", MaLinhVuc = 4, MoTa = "Xây dựng kho dữ liệu và truy vấn phân tích.", YeuCau = "SQL Server, Data Warehouse", SoSinhVienToiDa = 2, NamHoc = 2025, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Dự báo giá cổ phiếu", MaLinhVuc = 4, MoTa = "Phân tích chuỗi thời gian và dự báo biến động giá.", YeuCau = "Phân tích dữ liệu, học máy", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Website bệnh viện thông minh", MaLinhVuc = 1, MoTa = "Đặt lịch khám và quản lý hồ sơ bệnh nhân.", YeuCau = "ASP.NET Core MVC", SoSinhVienToiDa = 2, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Ứng dụng quản lý kho", MaLinhVuc = 1, MoTa = "Quản lý nhập xuất và tồn kho vật tư.", YeuCau = "ASP.NET Core MVC", SoSinhVienToiDa = 1, NamHoc = 2026, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.MoDangKy },
                    new DeTai { TenDeTai = "Hệ thống nhúng thu thập dữ liệu", MaLinhVuc = 5, MoTa = "Thu thập và lưu trữ dữ liệu cảm biến.", YeuCau = "Nhúng, cơ sở dữ liệu nhúng", SoSinhVienToiDa = 2, NamHoc = 2025, HocKy = 1, TrangThai = TrangThaiHeThong.DeTai.TamKhoa },
                    new DeTai { TenDeTai = "Nghiên cứu bảo mật hợp đồng thông minh", MaLinhVuc = 3, MoTa = "Bảo mật và theo dõi hợp đồng trên blockchain.", YeuCau = "Blockchain, hợp đồng số", SoSinhVienToiDa = 2, NamHoc = 2024, HocKy = 2, TrangThai = TrangThaiHeThong.DeTai.KetThuc });
                context.SaveChanges();
            }

            if (!context.DangKyDeTais.Any())
            {
                SeedDangKy(context);
            }

            if (!context.PhanCongHuongDans.Any())
            {
                SeedPhanCong(context);
            }

            DongBoTrangThaiDeTai(context);
        }

        /// <summary>
        /// Tạo 30 đăng ký đủ tình huống (mục 15).
        /// Bảo đảm: mỗi sinh viên có nhiều nhất 1 đăng ký "Đã duyệt" (luật mục 7.3).
        /// SV001 chỉ có đăng ký bị từ chối nên vẫn đăng ký đề tài mới được.
        /// </summary>
        private static void SeedDangKy(AppDbContext context)
        {
            // (mã sinh viên, mã đề tài, trạng thái, trạng thái thực hiện, ngày đăng ký, lý do)
            var duLieu = new (string sv, int deTai, string trangThai, string thucHien, DateTime ngay, string lyDo)[]
            {
                // 13 đăng ký Đã duyệt - mỗi sinh viên một đề tài
                ("SV002", 1, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 8, 12), "Đã thực hiện phần giao diện, muốn tiếp tục đồ án."),
                ("SV003", 2, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 8, 15), "Quan tâm tới thị giác máy tính và ứng dụng trong điểm danh."),
                ("SV004", 2, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 8, 18), "Muốn tham gia nhóm phát triển ứng dụng điểm danh AI."),
                ("SV005", 3, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.HoanThanh, new DateTime(2025, 8, 20), "Đã hoàn thành đồ án, muốn đăng ký đề tài mới để tiếp tục học kỳ sau."),
                ("SV006", 3, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.HoanThanh, new DateTime(2025, 8, 22), "Đã hoàn thành đồ án cũ, sẵn sàng làm đề tài mới."),
                ("SV007", 5, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 9, 2), "Đã có kinh nghiệm với xử lý ngôn ngữ tự nhiên."),
                ("SV008", 6, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2025, 9, 5), "Đã từng làm đề tài phân tích dữ liệu, muốn làm lại với dữ liệu lớn hơn."),
                ("SV009", 9, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 9, 8), "Muốn nghiên cứu giám sát và cảnh báo sự kiện mạng."),
                ("SV010", 9, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 9, 10), "Đã có kiến thức mạng căn bản, muốn mở rộng sang an ninh mạng."),
                ("SV011", 9, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 9, 12), "Sẽ hỗ trợ nhóm phân tích log và xây dựng quy tắc cảnh báo."),
                ("SV012", 11, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 9, 15), "Từng làm đồ án Arduino, muốn chuyển sang IoT hoàn chỉnh."),
                ("SV013", 13, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.HoanThanh, new DateTime(2025, 9, 18), "Đã nộp đồ án nền tảng học trực tuyến, muốn đăng ký đề tài tiếp theo."),
                ("SV014", 15, TrangThaiHeThong.DangKyDeTai.DaDuyet, TrangThaiHeThong.TrangThaiThucHien.DangThucHien, new DateTime(2025, 9, 20), "Muốn tìm hiểu về kho dữ liệu và truy vấn phân tích."),

                // 10 đăng ký Chờ duyệt - để màn hình duyệt của Admin có việc để làm
                ("SV015", 6, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 5), "Đã hoàn thành đồ án, muốn tiếp tục với đề tài phân tích dữ liệu."),
                ("SV016", 6, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 7), "Quan tâm tới khai phá dữ liệu mạng xã hội."),
                ("SV017", 11, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 9), "Đã làm phần cứng nhà thông minh, muốn phát triển tiếp phần mềm."),
                ("SV018", 13, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 12), "Muốn xây dựng nền tảng học trực tuyến cho trường."),
                ("SV019", 15, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 15), "Sẽ hỗ trợ thiết kế mô hình kho dữ liệu."),
                ("SV020", 12, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 18), "Đã làm cảm biến nhiệt độ, muốn mở rộng mạng cảm biến."),
                ("SV021", 13, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 2), "Muốn làm lại đề tài nền tảng học trực tuyến ở góc nhìn khác."),
                ("SV022", 16, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 5), "Đã học phân tích chuỗi thời gian, muốn áp dụng vào tài chính."),
                ("SV023", 17, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 8), "Muốn làm website bệnh viện thông minh cho đồ án chuyên ngành."),
                ("SV024", 18, TrangThaiHeThong.DangKyDeTai.ChoDuyet, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 10), "Đã làm đồ án quản lý kho, muốn cải thiện giao diện và hiệu năng."),

                // 3 đăng ký bị từ chối - có lý do để sinh viên biết cần điều chỉnh gì
                ("SV001", 1, TrangThaiHeThong.DangKyDeTai.TuChoi, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 1, 10), "Muốn thực hiện đồ án về quản lý thư viện."),
                ("SV025", 9, TrangThaiHeThong.DangKyDeTai.TuChoi, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 12), "Muốn tham gia đề tài giám sát an ninh mạng."),
                ("SV026", 20, TrangThaiHeThong.DangKyDeTai.TuChoi, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 15), "Quan tâm tới bảo mật hợp đồng thông minh."),

                // 4 đăng ký bị hủy
                ("SV027", 3, TrangThaiHeThong.DangKyDeTai.DaHuy, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 18), "Đăng ký tạm để giữ chỗ."),
                ("SV028", 6, TrangThaiHeThong.DangKyDeTai.DaHuy, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 20), "Cân nhắc lại sau khi đọc yêu cầu đề tài."),
                ("SV029", 13, TrangThaiHeThong.DangKyDeTai.DaHuy, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 22), "Đã đăng ký đề tài khác, tự hủy đăng ký này."),
                ("SV030", 16, TrangThaiHeThong.DangKyDeTai.DaHuy, TrangThaiHeThong.TrangThaiThucHien.ChuaBatDau, new DateTime(2026, 2, 25), "Không đủ thời gian trong học kỳ này.")
            };

            foreach (var item in duLieu)
            {
                context.DangKyDeTais.Add(new DangKyDeTai
                {
                    MaSinhVien = item.sv,
                    MaDeTai = item.deTai,
                    NgayDangKy = item.ngay,
                    LyDoDangKy = item.lyDo,
                    TrangThai = item.trangThai,
                    TrangThaiThucHien = item.thucHien,
                    GhiChuDuyet = GhiChuTheoTrangThai(item.trangThai, item.deTai)
                });
            }
            context.SaveChanges();
        }

        /// <summary>Ghi chú mẫu cho các đăng ký không còn ở trạng thái Chờ duyệt.</summary>
        private static string? GhiChuTheoTrangThai(string trangThai, int maDeTai) => trangThai switch
        {
            TrangThaiHeThong.DangKyDeTai.DaDuyet => "Hồ sơ hợp lệ, đề nghị liên hệ giảng viên để bắt đầu thực hiện.",
            TrangThaiHeThong.DangKyDeTai.TuChoi => maDeTai == 9
                ? "Đề tài đã đủ số lượng sinh viên khi đăng ký, vui lòng chọn đề tài khác."
                : maDeTai == 20
                    ? "Đề tài đã kết thúc trong học kỳ trước, không còn nhận sinh viên."
                    : "Đề tài đã đủ số lượng sinh viên, vui lòng chọn đề tài khác.",
            TrangThaiHeThong.DangKyDeTai.DaHuy => "Sinh viên tự hủy đăng ký.",
            _ => null
        };

        /// <summary>
        /// Tạo phân công giảng viên hướng dẫn (mục 15).
        /// Bảo đảm: giảng viên luôn đúng lĩnh vực của đề tài (mục 8.3)
        /// và tải hướng dẫn không vượt SoSinhVienHuongDanToiDa (mục 8.4).
        /// Có thêm 2 bản ghi lịch sử: một "Đã thay đổi" và một "Kết thúc".
        /// </summary>
        private static void SeedPhanCong(AppDbContext context)
        {
            // Lấy mã đăng ký theo cặp (sinh viên, đề tài) để không phụ thuộc thứ tự ID sinh ra
            var dk = context.DangKyDeTais.AsNoTracking().ToList();
            int LayMaDangKy(string maSv, int maDeTai)
                => dk.First(d => d.MaSinhVien == maSv && d.MaDeTai == maDeTai).MaDangKy;

            // 13 phân công hiện hành cho các đăng ký Đã duyệt
            var hienTai = new (string sv, int deTai, string gv, DateTime ngayBatDau)[]
            {
                ("SV002", 1,  "GV001", new DateTime(2025, 8, 15)), // lĩnh vực 1
                ("SV003", 2,  "GV003", new DateTime(2025, 8, 18)), // lĩnh vực 2
                ("SV004", 2,  "GV003", new DateTime(2025, 8, 20)), // GV003 đạt giới hạn 2/2
                ("SV005", 3,  "GV002", new DateTime(2025, 8, 22)), // lĩnh vực 1
                ("SV006", 3,  "GV002", new DateTime(2025, 8, 25)),
                ("SV007", 5,  "GV004", new DateTime(2025, 9, 3)),  // lĩnh vực 2
                ("SV008", 6,  "GV008", new DateTime(2026, 1, 5)),  // lĩnh vực 4
                ("SV009", 9,  "GV005", new DateTime(2025, 9, 9)),  // lĩnh vực 3
                ("SV010", 9,  "GV005", new DateTime(2025, 9, 11)),
                ("SV011", 9,  "GV006", new DateTime(2025, 9, 13)),
                ("SV012", 11, "GV009", new DateTime(2025, 9, 16)), // lĩnh vực 5
                ("SV013", 13, "GV002", new DateTime(2025, 9, 19)), // GV002 đạt giới hạn 3/3
                ("SV014", 15, "GV007", new DateTime(2026, 3, 1))  // lĩnh vực 4
            };

            foreach (var item in hienTai)
            {
                context.PhanCongHuongDans.Add(new PhanCongHuongDan
                {
                    MaDangKy = LayMaDangKy(item.sv, item.deTai),
                    MaGiangVien = item.gv,
                    NgayBatDau = item.ngayBatDau,
                    NgayKetThuc = null,
                    TrangThai = TrangThaiHeThong.PhanCongHuongDan.DangHuongDan,
                    GhiChu = "Bắt đầu hướng dẫn đồ án."
                });
            }

            // Lịch sử: SV008 được GV007 hướng dẫn học kỳ trước rồi kết thúc (mục 8.5)
            context.PhanCongHuongDans.Add(new PhanCongHuongDan
            {
                MaDangKy = LayMaDangKy("SV008", 6),
                MaGiangVien = "GV007",
                NgayBatDau = new DateTime(2025, 9, 6),
                NgayKetThuc = new DateTime(2025, 12, 31),
                TrangThai = TrangThaiHeThong.PhanCongHuongDan.KetThuc,
                GhiChu = "Kết thúc hướng dẫn do giảng viên nghỉ công tác."
            });

            // Lịch sử: SV014 đổi từ GV008 sang GV007 (mục 8.5)
            context.PhanCongHuongDans.Add(new PhanCongHuongDan
            {
                MaDangKy = LayMaDangKy("SV014", 15),
                MaGiangVien = "GV008",
                NgayBatDau = new DateTime(2026, 1, 5),
                NgayKetThuc = new DateTime(2026, 2, 28),
                TrangThai = TrangThaiHeThong.PhanCongHuongDan.DaThayDoi,
                GhiChu = "Thay đổi sang giảng viên GV007."
            });

            context.SaveChanges();
        }

        /// <summary>
        /// Cập nhật trạng thái đề tài theo số chỗ còn thực tế (mục 6.7).
        /// Đề tài Tạm khóa hoặc Kết thúc thì giữ nguyên trạng thái do Admin quyết định.
        /// </summary>
        private static void DongBoTrangThaiDeTai(AppDbContext context)
        {
            var deTais = context.DeTais.Include(d => d.DangKyDeTais).ToList();
            bool thayDoi = false;

            foreach (var deTai in deTais)
            {
                if (deTai.TrangThai is TrangThaiHeThong.DeTai.TamKhoa or TrangThaiHeThong.DeTai.KetThuc)
                {
                    continue;
                }

                int soDaDuyet = deTai.DangKyDeTais
                    .Count(d => d.TrangThai == TrangThaiHeThong.DangKyDeTai.DaDuyet);

                string trangThai = soDaDuyet >= deTai.SoSinhVienToiDa
                    ? TrangThaiHeThong.DeTai.DuSoLuong
                    : TrangThaiHeThong.DeTai.MoDangKy;

                if (deTai.TrangThai != trangThai)
                {
                    deTai.TrangThai = trangThai;
                    thayDoi = true;
                }
            }

            if (thayDoi)
            {
                context.SaveChanges();
            }
        }
    }
}