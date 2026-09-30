// Họ và tên: [Đoàn Quốc Hợp]
// Mã sinh viên: [23103100097]
// Nội dung thực hiện: Model Tài khoản (Module 1)

using System.ComponentModel.DataAnnotations;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    public class LinhVuc
    {
        [Key]
        public int MaLinhVuc { get; set; }

        [Required(ErrorMessage = "Tên lĩnh vực không được để trống")]
        [StringLength(100)]
        public string TenLinhVuc { get; set; }

        public string? MoTa { get; set; }

        [Required]
        public string TrangThai { get; set; } // HoatDong, NgungHoatDong

        // Quan hệ 1-n: 1 Lĩnh vực có nhiều Giảng viên và Đề tài
        public virtual ICollection<GiangVien> GiangViens { get; set; } = new List<GiangVien>();
        public virtual ICollection<DeTai> DeTais { get; set; } = new List<DeTai>();
    }
}