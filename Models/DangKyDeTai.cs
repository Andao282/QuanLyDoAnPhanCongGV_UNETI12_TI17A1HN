// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Model Đăng ký đề tài (Module 3)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class DangKyDeTai
    {
        [Key]
        public int MaDangKy { get; set; }

        [Required]
        [StringLength(20)]
        public string MaSinhVien { get; set; }

        public int MaDeTai { get; set; }

        [Required]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        public string? LyDoDangKy { get; set; }

        [Required]
        public string TrangThai { get; set; } // ChoDuyet, DaDuyet, TuChoi, DaHuy

        public string? GhiChuDuyet { get; set; }

        [ForeignKey("MaSinhVien")]
        public virtual SinhVien SinhVien { get; set; }

        [ForeignKey("MaDeTai")]
        public virtual DeTai DeTai { get; set; }

        // Quan hệ 1-n: 1 Đăng ký có nhiều Phân công (lưu lịch sử)
        public virtual ICollection<PhanCongHuongDan> PhanCongHuongDans { get; set; } = new List<PhanCongHuongDan>();
    }
}