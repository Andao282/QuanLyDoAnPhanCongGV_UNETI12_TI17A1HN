// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Các lớp chứa dữ liệu thống kê cho Dashboard và màn hình Thống kê (Module 4)
// Mục 8.7 và 8.8 yêu cầu các số liệu phải lấy trực tiếp từ Database bằng LINQ tại thời điểm truy vấn,
// vì vậy không lưu tổng số cố định, chỉ dùng các lớp này để hứng kết quả truy vấn.

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    /// <summary>Các con số hiển thị trên Dashboard (mục 8.7).</summary>
    public class DashboardSoLieu
    {
        public int TongLinhVuc { get; set; }
        public int TongGiangVien { get; set; }
        public int TongSinhVien { get; set; }
        public int TongDeTai { get; set; }

        public int SoDangKyChoDuyet { get; set; }
        public int SoDangKyDaDuyet { get; set; }

        public int SoDeTaiConCho { get; set; }
        public int SoDeTaiDuSoLuong { get; set; }

        public int SoSinhVienDangCoGiangVien { get; set; }
        public int SoGiangVienDatGioiHan { get; set; }
    }

    /// <summary>Số đề tài theo từng lĩnh vực (mục 8.8).</summary>
    public class ThongKeDeTaiTheoLinhVuc
    {
        public string TenLinhVuc { get; set; } = string.Empty;
        public int SoDeTai { get; set; }
    }

    /// <summary>Số đăng ký theo từng trạng thái (mục 8.8).</summary>
    public class ThongKeDangKyTheoTrangThai
    {
        public string TrangThai { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }

    /// <summary>Số sinh viên và số chỗ còn của từng đề tài (mục 8.8).</summary>
    public class ThongKeDeTaiChiTiet
    {
        public int MaDeTai { get; set; }
        public string TenDeTai { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public int NamHoc { get; set; }
        public int HocKy { get; set; }
        public int SoSinhVienToiDa { get; set; }
        public int SoDangKyHopLe { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        /// <summary>SoChoCon = SoSinhVienToiDa - SoDangKyHopLe (mục 25).</summary>
        public int SoChoCon => SoSinhVienToiDa - SoDangKyHopLe;
    }

    /// <summary>Tải hướng dẫn và tỷ lệ sử dụng khả năng hướng dẫn của từng giảng viên (mục 8.8).</summary>
    public class ThongKeTaiGiangVien
    {
        public string MaGiangVien { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string TenLinhVuc { get; set; } = string.Empty;
        public int SoSinhVienHuongDanToiDa { get; set; }
        public int SoDangHuongDanHienTai { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        /// <summary>Tỷ lệ sử dụng khả năng hướng dẫn, tính bằng phần trăm.</summary>
        public double TyLeSuDung => SoSinhVienHuongDanToiDa <= 0
            ? 0
            : Math.Round(SoDangHuongDanHienTai * 100.0 / SoSinhVienHuongDanToiDa, 1);
    }

    /// <summary>Số đăng ký theo từng tháng (mục 8.8).</summary>
    public class ThongKeDangKyTheoThang
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int SoLuong { get; set; }

        public string TenThang => $"Tháng {Thang}/{Nam}";
    }

    /// <summary>Số đề tài theo năm học / học kỳ (mục 8.8).</summary>
    public class ThongKeDeTaiTheoHocKy
    {
        public int NamHoc { get; set; }
        public int HocKy { get; set; }
        public int SoDeTai { get; set; }
    }
}