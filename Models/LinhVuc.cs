using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    [Table("LinhVucs")]
    public class LinhVuc
    {
        [Key]
        public int MaLinhVuc { get; set; }

        [Required(ErrorMessage = "Tên lĩnh vực là bắt buộc")]
        [StringLength(100)]
        public string? TenLinhVuc { get; set; } // Thêm dấu ? vào đây

        public string? MoTa { get; set; } // Thêm dấu ? vào đây

        [Required]
        public string? TrangThai { get; set; } // Thêm dấu ? vào đây
    }
}