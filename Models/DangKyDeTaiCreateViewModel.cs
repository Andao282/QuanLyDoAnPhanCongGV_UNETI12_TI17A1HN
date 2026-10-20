// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: ViewModel cho form Sinh viên đăng ký đề tài (Module 3)

using System.ComponentModel.DataAnnotations;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models
{
    // ViewModel chỉ chứa đúng những trường người dùng được phép gửi lên.
    // Các trường do hệ thống tự quyết định (MaSinhVien, NgayDangKy, TrangThai)
    // cố ý không nằm ở đây, nhờ vậy chúng không bị validate nhầm khi model binding.
    public class DangKyDeTaiCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn một đề tài.")]
        [Display(Name = "Chọn đề tài")]
        public int? MaDeTai { get; set; }

        [Display(Name = "Lý do đăng ký")]
        [StringLength(500, ErrorMessage = "Lý do đăng ký không được vượt quá {1} ký tự.")]
        public string? LyDoDangKy { get; set; }
    }
}