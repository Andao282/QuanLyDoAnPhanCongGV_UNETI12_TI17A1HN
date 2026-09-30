// Họ và tên: [Nguyễn Viết Cường]
// Mã sinh viên: [23103100072]
// Nội dung thực hiện: Model Giảng viên (Module 2)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class DeTai
    {
        [Key]
        public int MaDeTai { get; set; }

        [Required(ErrorMessage = "Tên đề tài không được để trống")]
        [StringLength(255)]
        public string TenDeTai { get; set; }

        public int MaLinhVuc { get; set; }

        public string? MoTa { get; set; }

        public string? YeuCau { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số sinh viên tối đa phải > 0")]
        public int SoSinhVienToiDa { get; set; }

        [Required]
        public int NamHoc { get; set; }

        [Required]
        public int HocKy { get; set; }

        [Required]
        public string TrangThai { get; set; } // MoDangKy, DuSoLuong, TamKhoa, KetThuc

        [ForeignKey("MaLinhVuc")]
        public virtual LinhVuc LinhVuc { get; set; }

        // Quan hệ 1-n: 1 Đề tài có nhiều Đăng ký
        public virtual ICollection<DangKyDeTai> DangKyDeTais { get; set; } = new List<DangKyDeTai>();
    }
}