// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Tập hợp tất cả trạng thái nghiệp vụ dùng chung cho toàn Project (Module 3 và Module 4)
// Theo mục 9.1 đề tài: "Các trường trạng thái phải dùng tập giá trị thống nhất trong toàn Project".
// Mục đích: không nhập trực tiếp chuỗi "ChoDuyet", "DaDuyet"... trong Controller/View,
// tránh sai chính tả và giúp sinh select + hiển thị tiếng Việt không dấu.

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Common
{
    public static class TrangThaiHeThong
    {
        // ==================== Vai trò ====================
        public static class VaiTro
        {
            public const string Admin = "Admin";
            public const string SinhVien = "SinhVien";

            public static readonly IReadOnlyList<string> TatCa = new[] { Admin, SinhVien };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                Admin => "Admin",
                SinhVien => "Sinh viên",
                _ => "-"
            };
        }

        // ==================== Tài khoản ====================
        public static class TaiKhoan
        {
            public const string HoatDong = "HoatDong";
            public const string Khoa = "Khoa";

            public static readonly IReadOnlyList<string> TatCa = new[] { HoatDong, Khoa };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                HoatDong => "Hoạt động",
                Khoa => "Đã khóa",
                _ => "-"
            };
        }

        // ==================== Lĩnh vực ====================
        public static class LinhVuc
        {
            public const string HoatDong = "HoatDong";
            public const string NgungHoatDong = "NgungHoatDong";

            public static readonly IReadOnlyList<string> TatCa = new[] { HoatDong, NgungHoatDong };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                HoatDong => "Hoạt động",
                NgungHoatDong => "Ngừng hoạt động",
                _ => "-"
            };
        }

        // ==================== Giảng viên (mục 6.1) ====================
        public static class GiangVien
        {
            public const string DangCongTac = "DangCongTac";
            public const string TamNgungHuongDan = "TamNgungHuongDan";

            public static readonly IReadOnlyList<string> TatCa = new[] { DangCongTac, TamNgungHuongDan };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                DangCongTac => "Đang công tác",
                TamNgungHuongDan => "Tạm ngừng hướng dẫn",
                _ => "-"
            };
        }

        // ==================== Sinh viên (mục 7.1) ====================
        public static class SinhVien
        {
            public const string HoatDong = "HoatDong";
            public const string DinhChi = "DinhChi";
            public const string DaTotNghiep = "DaTotNghiep";

            public static readonly IReadOnlyList<string> TatCa = new[] { HoatDong, DinhChi, DaTotNghiep };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                HoatDong => "Hoạt động",
                DinhChi => "Đình chỉ",
                DaTotNghiep => "Đã tốt nghiệp",
                _ => "-"
            };
        }

        // ==================== Đề tài (mục 6.2) ====================
        public static class DeTai
        {
            public const string MoDangKy = "MoDangKy";
            public const string DuSoLuong = "DuSoLuong";
            public const string TamKhoa = "TamKhoa";
            public const string KetThuc = "KetThuc";

            public static readonly IReadOnlyList<string> TatCa = new[] { MoDangKy, DuSoLuong, TamKhoa, KetThuc };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                MoDangKy => "Mở đăng ký",
                DuSoLuong => "Đủ số lượng",
                TamKhoa => "Tạm khóa",
                KetThuc => "Kết thúc",
                _ => "-"
            };
        }

        // ==================== Đăng ký đề tài (mục 7.2) ====================
        public static class DangKyDeTai
        {
            public const string ChoDuyet = "ChoDuyet";
            public const string DaDuyet = "DaDuyet";
            public const string TuChoi = "TuChoi";
            public const string DaHuy = "DaHuy";

            public static readonly IReadOnlyList<string> TatCa = new[] { ChoDuyet, DaDuyet, TuChoi, DaHuy };

            // Mục 6.7: chỉ đăng ký "Đã duyệt" mới chiếm chỗ.
            // Từ chối / Đã hủy không tính vào số lượng đang chiếm chỗ.
            public static readonly IReadOnlyList<string> DangKyHopLe = new[] { DaDuyet };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                ChoDuyet => "Chờ duyệt",
                DaDuyet => "Đã duyệt",
                TuChoi => "Từ chối",
                DaHuy => "Đã hủy",
                _ => "-"
            };
        }

        // ==================== Trạng thái thực hiện (mục 8.6) ====================
        public static class TrangThaiThucHien
        {
            public const string ChuaBatDau = "ChuaBatDau";
            public const string DangThucHien = "DangThucHien";
            public const string HoanThanh = "HoanThanh";

            public static readonly IReadOnlyList<string> TatCa = new[] { ChuaBatDau, DangThucHien, HoanThanh };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                ChuaBatDau => "Chưa bắt đầu",
                DangThucHien => "Đang thực hiện",
                HoanThanh => "Hoàn thành",
                _ => "-"
            };
        }

        // ==================== Phân công hướng dẫn (mục 8.2) ====================
        public static class PhanCongHuongDan
        {
            public const string DangHuongDan = "DangHuongDan";
            public const string DaThayDoi = "DaThayDoi";
            public const string KetThuc = "KetThuc";

            public static readonly IReadOnlyList<string> TatCa = new[] { DangHuongDan, DaThayDoi, KetThuc };

            // Mục 8.4: tải hiện tại = số phân công có trạng thái Đang hướng dẫn.
            public static readonly IReadOnlyList<string> DangHieuLuc = new[] { DangHuongDan };

            public static string HienThi(string? giaTri) => giaTri switch
            {
                DangHuongDan => "Đang hướng dẫn",
                DaThayDoi => "Đã thay đổi",
                KetThuc => "Kết thúc",
                _ => "-"
            };
        }
    }
}