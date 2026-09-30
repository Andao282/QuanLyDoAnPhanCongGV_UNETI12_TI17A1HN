// Họ và tên: [Đoàn Quốc Hợp]
// Mã sinh viên: [23103100097]
// Nội dung thực hiện: Model Tài khoản (Module 1)

using System.ComponentModel.DataAnnotations;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255)]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string VaiTro { get; set; } // Admin, SinhVien

        [Required]
        public string TrangThai { get; set; } // HoatDong, Khoa

        // Quan hệ 1-1 với SinhVien
        public virtual SinhVien SinhVien { get; set; }
    }
}