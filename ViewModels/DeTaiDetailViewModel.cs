using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.ViewModels
{
    public class DeTaiDetailViewModel
    {
        public DeTai DeTai { get; set; } = null!;
        public int SoSinhVienDangKyHopLe { get; set; }
        public int SoChoCon { get; set; }
        public bool IsHetCho => SoChoCon <= 0;
        public bool IsLoiNghiepVu => SoChoCon < 0;

        public List<DangKyItemViewModel> DanhSachDangKy { get; set; } = new();
    }

    public class DangKyItemViewModel
    {
        public int MaDangKy { get; set; }
        public string MaSinhVien { get; set; } = string.Empty;
        public string HoTenSinhVien { get; set; } = string.Empty;
        public string Lop { get; set; } = string.Empty;
        public DateTime NgayDangKy { get; set; }
        public string? LyDoDangKy { get; set; }
        public string TrangThai { get; set; } = string.Empty; // ChoDuyet, DaDuyet, TuChoi, DaHuy
        public string? GhiChuDuyet { get; set; }
        public bool IsHopLeTinhCho { get; set; } // true nếu DaDuyet
    }
}
