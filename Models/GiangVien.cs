// Họ và tên: [Nguyễn Viết Cường]
// Mã sinh viên: [23103100072]
// Nội dung thực hiện: Model Giảng viên (Module 2)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class GiangVien
    {
        [Key]
        [StringLength(20)]
        public string MaGiangVien { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(50)]
        public string? HocVi { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        public int MaLinhVuc { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số sinh viên hướng dẫn tối đa phải > 0")]
        public int SoSinhVienHuongDanToiDa { get; set; }

        [Required]
        public string TrangThai { get; set; } // DangCongTac, TamNgungHuongDan

        [ForeignKey("MaLinhVuc")]
        public virtual LinhVuc LinhVuc { get; set; }

        // Quan hệ 1-n: 1 Giảng viên có nhiều Phân công hướng dẫn
        public virtual ICollection<PhanCongHuongDan> PhanCongHuongDans { get; set; } = new List<PhanCongHuongDan>();
    }
}