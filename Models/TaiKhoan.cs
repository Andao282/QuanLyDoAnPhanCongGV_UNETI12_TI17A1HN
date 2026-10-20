using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    [Table("TaiKhoans")]
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50)]
        public string? TenDangNhap { get; set; } // Thêm dấu ? vào đây

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(255)]
        public string? MatKhau { get; set; } // Thêm dấu ? vào đây

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        public string? HoTen { get; set; } // Thêm dấu ? vào đây

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string? Email { get; set; } // Thêm dấu ? vào đây

        [Required]
        public string? VaiTro { get; set; } // Thêm dấu ? vào đây

        [Required]
        public string? TrangThai { get; set; } // Thêm dấu ? vào đây
    }
}