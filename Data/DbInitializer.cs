using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Đảm bảo Database đã được tạo
            context.Database.EnsureCreated();

            // Nếu đã có dữ liệu đề tài thì không seed lại
            if (context.DeTais.Any())
            {
                return;
            }

            // 1. Seed Lĩnh vực
            var linhVucAI = new LinhVuc { TenLinhVuc = "Trí tuệ nhân tạo (AI & Data)", MoTa = "Machine Learning, Deep Learning, Computer Vision, NLP", TrangThai = "HoatDong" };
            var linhVucWeb = new LinhVuc { TenLinhVuc = "Phát triển Web & Cloud", MoTa = "ASP.NET Core, React, Vue, Cloud Computing", TrangThai = "HoatDong" };
            var linhVucMobile = new LinhVuc { TenLinhVuc = "Lập trình Di động (Mobile)", MoTa = "Flutter, React Native, Android, iOS", TrangThai = "HoatDong" };
            var linhVucIoT = new LinhVuc { TenLinhVuc = "IoT & Hệ thống nhúng", MoTa = "ESP32, Arduino, Raspberry Pi, MQTT, LoRa", TrangThai = "HoatDong" };
            var linhVucATTT = new LinhVuc { TenLinhVuc = "Mạng & An toàn thông tin", MoTa = "Cybersecurity, SOC, Pentest, Network Security", TrangThai = "HoatDong" };

            context.LinhVucs.AddRange(linhVucAI, linhVucWeb, linhVucMobile, linhVucIoT, linhVucATTT);
            context.SaveChanges();

            // 2. Seed Tài khoản & Sinh viên mẫu
            var sinhViens = new List<SinhVien>();
            for (int i = 1; i <= 10; i++)
            {
                var tk = new TaiKhoan
                {
                    TenDangNhap = $"sv{i:D2}",
                    MatKhau = "123456",
                    HoTen = $"Sinh Viên UNETI {i:D2}",
                    Email = $"sv{i:D2}@uneti.edu.vn",
                    VaiTro = "SinhVien",
                    TrangThai = "HoatDong"
                };
                context.TaiKhoans.Add(tk);
                context.SaveChanges();

                var sv = new SinhVien
                {
                    MaSinhVien = $"231031000{i:D2}",
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = tk.HoTen,
                    NgaySinh = new DateTime(2004, 1, 1).AddMonths(i),
                    Lop = "TI17A1HN",
                    Khoa = "Công nghệ thông tin",
                    ChuyenNganh = "Kỹ thuật phần mềm",
                    Email = tk.Email,
                    SoDienThoai = $"09876543{i:D2}",
                    TrangThai = "HoatDong"
                };
                sinhViens.Add(sv);
            }
            context.SinhViens.AddRange(sinhViens);
            context.SaveChanges();

            // 3. Seed Danh sách Đề tài phong phú phục vụ kiểm thử 6.3 -> 6.8
            var dt1 = new DeTai
            {
                TenDeTai = "Hệ thống nhận diện biển số xe máy thông minh dùng YOLOv8",
                MaLinhVuc = linhVucAI.MaLinhVuc,
                MoTa = "Ứng dụng thị giác máy tính nhận diện biển số phương tiện ra vào bãi xe UNETI",
                YeuCau = "Thành thạo Python, PyTorch, OpenCV, kinh nghiệm xử lý hình ảnh",
                SoSinhVienToiDa = 2,
                NamHoc = 2025,
                HocKy = 1,
                TrangThai = "MoDangKy"
            };

            var dt2 = new DeTai
            {
                TenDeTai = "Xây dựng website thương mại điện tử bán sách với ASP.NET Core & React",
                MaLinhVuc = linhVucWeb.MaLinhVuc,
                MoTa = "Website tích hợp thanh toán VnPay, quản lý kho, giỏ hàng trực tuyến",
                YeuCau = "Thành thạo C# ASP.NET Core, ReactJS, Entity Framework Core",
                SoSinhVienToiDa = 3,
                NamHoc = 2025,
                HocKy = 1,
                TrangThai = "MoDangKy"
            };

            var dt3 = new DeTai
            {
                TenDeTai = "Ứng dụng điểm danh sinh viên thông minh bằng Flutter & Firebase",
                MaLinhVuc = linhVucMobile.MaLinhVuc,
                MoTa = "App di động định vị GPS và quét mã QR động để điểm danh lớp học",
                YeuCau = "Lập trình Dart, Flutter, tích hợp Firebase Realtime Database",
                SoSinhVienToiDa = 2,
                NamHoc = 2025,
                HocKy = 2,
                TrangThai = "MoDangKy"
            };

            var dt4 = new DeTai
            {
                TenDeTai = "Hệ thống giám sát chất lượng không khí phòng học bằng ESP32 và MQTT",
                MaLinhVuc = linhVucIoT.MaLinhVuc,
                MoTa = "Thu thập dữ liệu nồng độ CO2, bụi mịn PM2.5, gửi cảnh báo qua Telegram",
                YeuCau = "Kiến thức vi điều khiển ESP32, lập trình C++, giao thức MQTT",
                SoSinhVienToiDa = 1,
                NamHoc = 2026,
                HocKy = 1,
                TrangThai = "MoDangKy"
            };

            // Đề tài demo trường hợp lỗi nghiệp vụ (6.7: SoChoCon < 0)
            var dt5 = new DeTai
            {
                TenDeTai = "Hệ thống phân tích cảm xúc phản hồi sinh viên bằng mô hình PhoBERT",
                MaLinhVuc = linhVucAI.MaLinhVuc,
                MoTa = "Đề tài demo kiểm tra phát hiện lỗi nghiệp vụ khi duyệt quá số lượng tối đa",
                YeuCau = "NLP cơ bản, mô hình Transformers",
                SoSinhVienToiDa = 1,
                NamHoc = 2025,
                HocKy = 1,
                TrangThai = "MoDangKy"
            };

            var dt6 = new DeTai
            {
                TenDeTai = "Phát hiện và cảnh báo sớm tấn công mã độc Ransomware dựa trên hành vi",
                MaLinhVuc = linhVucATTT.MaLinhVuc,
                MoTa = "Phân tích log hệ điều hành Windows, phát hiện bất thường bằng mô hình Random Forest",
                YeuCau = "Kiến thức an toàn thông tin, phân tích mã độc, Python Scikit-learn",
                SoSinhVienToiDa = 2,
                NamHoc = 2025,
                HocKy = 2,
                TrangThai = "TamKhoa"
            };

            var dt7 = new DeTai
            {
                TenDeTai = "Ứng dụng quản lý lịch học và đồ án khoa CNTT UNETI trên nền tảng Web",
                MaLinhVuc = linhVucWeb.MaLinhVuc,
                MoTa = "Hệ thống quản lý phân công giảng viên và sinh viên thực hiện đồ án tốt nghiệp",
                YeuCau = "ASP.NET Core MVC, SQL Server, HTML5/CSS3",
                SoSinhVienToiDa = 4,
                NamHoc = 2026,
                HocKy = 1,
                TrangThai = "MoDangKy"
            };

            var dt8 = new DeTai
            {
                TenDeTai = "Hệ thống Chatbot tư vấn quy chế tuyển sinh đại học bằng RAG và LLM",
                MaLinhVuc = linhVucAI.MaLinhVuc,
                MoTa = "Tích hợp Retrieval Augmented Generation với mô hình ngôn ngữ lớn nguồn mở",
                YeuCau = "Hiểu biết về Vector Database, LangChain, LlamaIndex",
                SoSinhVienToiDa = 3,
                NamHoc = 2026,
                HocKy = 1,
                TrangThai = "MoDangKy"
            };

            var dt9 = new DeTai
            {
                TenDeTai = "Hệ thống tưới cây tự động trong nông nghiệp thông minh qua mạng LoRaWAN",
                MaLinhVuc = linhVucIoT.MaLinhVuc,
                MoTa = "Đo độ ẩm đất, điều khiển máy bơm từ xa khoảng cách lớn dùng sóng LoRa",
                YeuCau = "Phần cứng Arduino/LoRa, thiết kế mạch cơ bản",
                SoSinhVienToiDa = 2,
                NamHoc = 2025,
                HocKy = 1,
                TrangThai = "KetThuc"
            };

            var dt10 = new DeTai
            {
                TenDeTai = "Xây dựng ứng dụng thanh toán không tiền mặt trên nền tảng Android",
                MaLinhVuc = linhVucMobile.MaLinhVuc,
                MoTa = "Ví điện tử mini hỗ trợ chuyển tiền, thanh toán bằng mã VietQR",
                YeuCau = "Lập trình Kotlin/Android SDK, tích hợp API ngân hàng",
                SoSinhVienToiDa = 2,
                NamHoc = 2026,
                HocKy = 2,
                TrangThai = "MoDangKy"
            };

            context.DeTais.AddRange(dt1, dt2, dt3, dt4, dt5, dt6, dt7, dt8, dt9, dt10);
            context.SaveChanges();

            // 4. Seed Đăng ký đề tài với các trạng thái khác nhau để kiểm tra tính số chỗ còn (6.7)
            // dt1: Tối đa 2 -> Có 1 DaDuyet, 1 ChoDuyet => Hợp lệ = 1, Còn chỗ = 2 - 1 = 1
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[0].MaSinhVien,
                MaDeTai = dt1.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-5),
                LyDoDangKy = "Em đam mê xử lý ảnh và Computer Vision",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đạt yêu cầu kiến thức nền tảng"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[1].MaSinhVien,
                MaDeTai = dt1.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-2),
                LyDoDangKy = "Muốn học hỏi thêm YOLOv8",
                TrangThai = "ChoDuyet",
                GhiChuDuyet = null
            });

            // dt2: Tối đa 3 -> Có 3 DaDuyet, 1 TuChoi => Hợp lệ = 3, Còn chỗ = 3 - 3 = 0 (Hết chỗ)
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[2].MaSinhVien,
                MaDeTai = dt2.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-10),
                LyDoDangKy = "Đã có kinh nghiệm làm web thương mại",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Được duyệt"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[3].MaSinhVien,
                MaDeTai = dt2.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-8),
                LyDoDangKy = "Muốn phát triển Backend ASP.NET",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Được duyệt"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[4].MaSinhVien,
                MaDeTai = dt2.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-6),
                LyDoDangKy = "Thành thạo Frontend React",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Được duyệt"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[5].MaSinhVien,
                MaDeTai = dt2.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-1),
                LyDoDangKy = "Đăng ký thêm",
                TrangThai = "TuChoi", // Không tính vào số chiếm chỗ
                GhiChuDuyet = "Đề tài đã đủ số lượng sinh viên"
            });

            // dt4: Tối đa 1 -> Có 1 DaDuyet => Hợp lệ = 1, Còn chỗ = 1 - 1 = 0 (Hết chỗ)
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[6].MaSinhVien,
                MaDeTai = dt4.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-4),
                LyDoDangKy = "Yêu thích phần cứng và IoT",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Duyệt"
            });

            // dt5: Tối đa 1 -> Có 2 DaDuyet => Hợp lệ = 2, Còn chỗ = 1 - 2 = -1 (LỖI NGHIỆP VỤ)
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[7].MaSinhVien,
                MaDeTai = dt5.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-7),
                LyDoDangKy = "Em muốn nghiên cứu PhoBERT",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đã duyệt"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[8].MaSinhVien,
                MaDeTai = dt5.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-6),
                LyDoDangKy = "Em cũng muốn làm PhoBERT",
                TrangThai = "DaDuyet", // Lỗi nghiệp vụ do duyệt thừa
                GhiChuDuyet = "Duyệt nhầm vượt sĩ số"
            });

            // dt7: Tối đa 4 -> Có 2 DaDuyet, 1 DaHuy => Hợp lệ = 2, Còn chỗ = 4 - 2 = 2
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[0].MaSinhVien,
                MaDeTai = dt7.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-3),
                LyDoDangKy = "Muốn tham gia dự án phần mềm trường",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đã duyệt"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[1].MaSinhVien,
                MaDeTai = dt7.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-3),
                LyDoDangKy = "Đăng ký cùng bạn",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đã duyệt"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[9].MaSinhVien,
                MaDeTai = dt7.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-4),
                LyDoDangKy = "Trùng lịch nên xin hủy",
                TrangThai = "DaHuy", // Không tính vào số chiếm chỗ
                GhiChuDuyet = "Sinh viên tự hủy"
            });

            // dt8: Tối đa 3 -> 1 DaDuyet => Hợp lệ = 1, Còn chỗ = 3 - 1 = 2
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[2].MaSinhVien,
                MaDeTai = dt8.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-2),
                LyDoDangKy = "Đam mê LLM RAG",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đã duyệt"
            });

            // dt9: Tối đa 2 -> 2 DaDuyet => Hợp lệ = 2, Còn chỗ = 0 (Kết thúc)
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[3].MaSinhVien,
                MaDeTai = dt9.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-30),
                LyDoDangKy = "Hoàn thành nghiên cứu LoRa",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đã nghiệm thu"
            });
            context.DangKyDeTais.Add(new DangKyDeTai
            {
                MaSinhVien = sinhViens[4].MaSinhVien,
                MaDeTai = dt9.MaDeTai,
                NgayDangKy = DateTime.Now.AddDays(-30),
                LyDoDangKy = "Tham gia thiết kế phần cứng",
                TrangThai = "DaDuyet",
                GhiChuDuyet = "Đã nghiệm thu"
            });

            context.SaveChanges();
        }
    }
}
