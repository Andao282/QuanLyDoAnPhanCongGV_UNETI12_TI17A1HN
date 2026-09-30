// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Model Phân công hướng dẫn (Module 4)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class PhanCongHuongDan
    {
        [Key]
        public int MaPhanCong { get; set; }

        public int MaDangKy { get; set; }

        [Required]
        [StringLength(20)]
        public string MaGiangVien { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        [Required]
        public string TrangThai { get; set; } // DangHuongDan, DaThayDoi, KetThuc

        public string? GhiChu { get; set; }

        [ForeignKey("MaDangKy")]
        public virtual DangKyDeTai DangKyDeTai { get; set; }

        [ForeignKey("MaGiangVien")]
        public virtual GiangVien GiangVien { get; set; }
    }
}