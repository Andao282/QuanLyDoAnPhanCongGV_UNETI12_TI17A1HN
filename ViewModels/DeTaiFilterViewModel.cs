using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.ViewModels
{
    public class DeTaiFilterViewModel
    {
        // 6.3. Tìm kiếm
        public string? SearchTerm { get; set; }

        // 6.4. Bộ lọc
        public int? MaLinhVuc { get; set; }
        public int? NamHoc { get; set; }
        public int? HocKy { get; set; }
        public string? TrangThai { get; set; } // MoDangKy, DuSoLuong, TamKhoa, KetThuc
        public string? TinhTrangCho { get; set; } // "ConCho", "HetCho"

        // 6.5. Sắp xếp
        public string? SortOrder { get; set; } // ten_asc, ten_desc, sv_asc, sv_desc, choCon_asc, choCon_desc

        // 6.6. Phân trang
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Danh sách đề tài sau khi phân trang
        public List<DeTaiViewModel> Items { get; set; } = new();

        // Dữ liệu cho các dropdown bộ lọc
        public List<SelectListItem> LinhVucList { get; set; } = new();
        public List<int> NamHocList { get; set; } = new();
        public List<int> HocKyList { get; set; } = new() { 1, 2, 3 };
    }
}
