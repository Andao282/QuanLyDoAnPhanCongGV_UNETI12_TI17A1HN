namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.ViewModels
{
    public class DeTaiViewModel
    {
        public int MaDeTai { get; set; }
        public string TenDeTai { get; set; } = string.Empty;
        public int MaLinhVuc { get; set; }
        public string TenLinhVuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string? YeuCau { get; set; }
        public int SoSinhVienToiDa { get; set; }
        public int NamHoc { get; set; }
        public int HocKy { get; set; }
        public string TrangThai { get; set; } = string.Empty; // MoDangKy, DuSoLuong, TamKhoa, KetThuc

        /// <summary>
        /// 6.7: Số sinh viên đã đăng ký hợp lệ (Trạng thái Đã duyệt / đang có hiệu lực)
        /// Các đăng ký Từ chối hoặc Đã hủy không được tính.
        /// </summary>
        public int SoSinhVienDangKyHopLe { get; set; }

        /// <summary>
        /// 6.7: SoChoCon = SoSinhVienToiDa - SoSinhVienDangKyHopLe
        /// Không cho Admin nhập trực tiếp. Phải tính từ dữ liệu thực tế.
        /// </summary>
        public int SoChoCon { get; set; }

        /// <summary>
        /// Nếu SoChoCon = 0, đề tài hết chỗ
        /// </summary>
        public bool IsHetCho => SoChoCon <= 0;

        /// <summary>
        /// 6.7: Nếu SoChoCon < 0, dữ liệu có lỗi nghiệp vụ và phải kiểm tra lại quá trình duyệt.
        /// </summary>
        public bool IsLoiNghiepVu => SoChoCon < 0;
    }
}
