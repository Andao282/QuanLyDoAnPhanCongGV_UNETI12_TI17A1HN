// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Model Sinh Viên (Module 3)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class SinhVien
    {
        [Key]
        [StringLength(20)]
        public string MaSinhVien { get; set; }

        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(50)]
        public string? Lop { get; set; }

        [StringLength(100)]
        public string? Khoa { get; set; }

        [StringLength(100)]
        public string? ChuyenNganh { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        [Required]
        public string TrangThai { get; set; } // HoatDong, DinhChi, DaTotNghiep

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan TaiKhoan { get; set; }

        // Quan hệ 1-n: 1 Sinh viên có nhiều Đăng ký đề tài
        public virtual ICollection<DangKyDeTai> DangKyDeTais { get; set; } = new List<DangKyDeTai>();
    }
}