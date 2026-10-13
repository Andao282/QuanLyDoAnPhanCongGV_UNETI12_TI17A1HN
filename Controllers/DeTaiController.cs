using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.ViewModels;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class DeTaiController : Controller
    {
        private readonly AppDbContext _context;

        public DeTaiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Thực hiện theo chuẩn quy trình:
        /// Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang -> Hiển thị
        /// </summary>
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? maLinhVuc,
            int? namHoc,
            int? hocKy,
            string? trangThai,
            string? tinhTrangCho,
            string? sortOrder,
            int page = 1)
        {
            // 1. TRUY VẤN BAN ĐẦU (Base query)
            // 6.7: SoChoCon = SoSinhVienToiDa - SoSinhVienDangKyHopLe
            // Đăng ký hợp lệ gồm các trạng thái "DaDuyet" (đã được duyệt/đang thực hiện).
            // Các đăng ký "TuChoi" hoặc "DaHuy" không được tính vào số lượng đang chiếm chỗ.
            var baseQuery = _context.DeTais
                .AsNoTracking()
                .Select(dt => new DeTaiViewModel
                {
                    MaDeTai = dt.MaDeTai,
                    TenDeTai = dt.TenDeTai,
                    MaLinhVuc = dt.MaLinhVuc,
                    TenLinhVuc = dt.LinhVuc != null ? dt.LinhVuc.TenLinhVuc : string.Empty,
                    MoTa = dt.MoTa,
                    YeuCau = dt.YeuCau,
                    SoSinhVienToiDa = dt.SoSinhVienToiDa,
                    NamHoc = dt.NamHoc,
                    HocKy = dt.HocKy,
                    TrangThai = dt.TrangThai,
                    SoSinhVienDangKyHopLe = dt.DangKyDeTais.Count(dk => dk.TrangThai == "DaDuyet"),
                    SoChoCon = dt.SoSinhVienToiDa - dt.DangKyDeTais.Count(dk => dk.TrangThai == "DaDuyet")
                });

            // 2. TÌM KIẾM (Search) - Mục 6.3
            // • Tìm theo tên đề tài.
            // • Tìm theo từ khóa trong mô tả hoặc yêu cầu.
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string keyword = searchTerm.Trim();
                baseQuery = baseQuery.Where(dt =>
                    dt.TenDeTai.Contains(keyword) ||
                    (dt.MoTa != null && dt.MoTa.Contains(keyword)) ||
                    (dt.YeuCau != null && dt.YeuCau.Contains(keyword)));
            }

            // 3. LỌC (Filter) - Mục 6.4 & 6.8
            // Có khả năng kết hợp tất cả các bộ lọc với từ khóa tìm kiếm
            // • Lĩnh vực
            if (maLinhVuc.HasValue && maLinhVuc.Value > 0)
            {
                baseQuery = baseQuery.Where(dt => dt.MaLinhVuc == maLinhVuc.Value);
            }

            // • Năm học
            if (namHoc.HasValue && namHoc.Value > 0)
            {
                baseQuery = baseQuery.Where(dt => dt.NamHoc == namHoc.Value);
            }

            // • Học kỳ
            if (hocKy.HasValue && hocKy.Value > 0)
            {
                baseQuery = baseQuery.Where(dt => dt.HocKy == hocKy.Value);
            }

            // • Trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                baseQuery = baseQuery.Where(dt => dt.TrangThai == trangThai);
            }

            // • Còn chỗ / Hết chỗ (Dựa trên số chỗ còn tính thực tế, không dựa trên trường nhập tay)
            if (!string.IsNullOrWhiteSpace(tinhTrangCho))
            {
                if (tinhTrangCho.Equals("ConCho", StringComparison.OrdinalIgnoreCase))
                {
                    baseQuery = baseQuery.Where(dt => dt.SoChoCon > 0);
                }
                else if (tinhTrangCho.Equals("HetCho", StringComparison.OrdinalIgnoreCase))
                {
                    baseQuery = baseQuery.Where(dt => dt.SoChoCon <= 0);
                }
            }

            // 4. SẮP XẾP (Sort) - Mục 6.5 & 6.8
            // • Tên đề tài A -> Z / Z -> A
            // • Số sinh viên tối đa tăng / giảm
            // • Số chỗ còn tăng / giảm (dựa trên dữ liệu đăng ký thực tế)
            baseQuery = sortOrder switch
            {
                "ten_asc" => baseQuery.OrderBy(dt => dt.TenDeTai),
                "ten_desc" => baseQuery.OrderByDescending(dt => dt.TenDeTai),
                "sv_asc" => baseQuery.OrderBy(dt => dt.SoSinhVienToiDa),
                "sv_desc" => baseQuery.OrderByDescending(dt => dt.SoSinhVienToiDa),
                "choCon_asc" => baseQuery.OrderBy(dt => dt.SoChoCon),
                "choCon_desc" => baseQuery.OrderByDescending(dt => dt.SoChoCon),
                _ => baseQuery.OrderByDescending(dt => dt.MaDeTai) // Mặc định: Mới nhất
            };

            // 5. PHÂN TRANG (Pagination) - Mục 6.6
            // Bắt buộc thực hiện phân trang bằng truy vấn EF Core/LINQ (.Skip().Take())
            int totalItems = await baseQuery.CountAsync();
            int pageSize = 6; // 6 đề tài trên 1 trang để giao diện lưới 3 cột x 2 dòng cân đối, đẹp mắt
            int pageIndex = page < 1 ? 1 : page;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages > 0 && pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }

            var items = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy danh sách Lĩnh vực và Năm học phục vụ bộ lọc
            var linhVucs = await _context.LinhVucs
                .AsNoTracking()
                .OrderBy(l => l.TenLinhVuc)
                .Select(l => new SelectListItem
                {
                    Value = l.MaLinhVuc.ToString(),
                    Text = l.TenLinhVuc
                })
                .ToListAsync();

            var namHocList = await _context.DeTais
                .AsNoTracking()
                .Select(d => d.NamHoc)
                .Distinct()
                .OrderByDescending(n => n)
                .ToListAsync();

            if (!namHocList.Any())
            {
                namHocList = new List<int> { DateTime.Now.Year, DateTime.Now.Year - 1 };
            }

            // 6. HIỂN THỊ (Display)
            var viewModel = new DeTaiFilterViewModel
            {
                SearchTerm = searchTerm,
                MaLinhVuc = maLinhVuc,
                NamHoc = namHoc,
                HocKy = hocKy,
                TrangThai = trangThai,
                TinhTrangCho = tinhTrangCho,
                SortOrder = sortOrder,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = items,
                LinhVucList = linhVucs,
                NamHocList = namHocList,
                HocKyList = new List<int> { 1, 2, 3 }
            };

            return View(viewModel);
        }

        /// <summary>
        /// Xem chi tiết đề tài và danh sách đăng ký thực tế để kiểm tra tính hợp lệ
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var deTai = await _context.DeTais
                .Include(d => d.LinhVuc)
                .Include(d => d.DangKyDeTais)
                    .ThenInclude(dk => dk.SinhVien)
                .FirstOrDefaultAsync(d => d.MaDeTai == id);

            if (deTai == null)
            {
                return NotFound();
            }

            // Tính số lượng hợp lệ (Đã duyệt)
            int soSinhVienDangKyHopLe = deTai.DangKyDeTais.Count(dk => dk.TrangThai == "DaDuyet");
            int soChoCon = deTai.SoSinhVienToiDa - soSinhVienDangKyHopLe;

            var danhSachDangKy = deTai.DangKyDeTais
                .OrderByDescending(dk => dk.NgayDangKy)
                .Select(dk => new DangKyItemViewModel
                {
                    MaDangKy = dk.MaDangKy,
                    MaSinhVien = dk.MaSinhVien,
                    HoTenSinhVien = dk.SinhVien != null ? dk.SinhVien.HoTen : dk.MaSinhVien,
                    Lop = dk.SinhVien?.Lop ?? "N/A",
                    NgayDangKy = dk.NgayDangKy,
                    LyDoDangKy = dk.LyDoDangKy,
                    TrangThai = dk.TrangThai,
                    GhiChuDuyet = dk.GhiChuDuyet,
                    IsHopLeTinhCho = dk.TrangThai == "DaDuyet"
                })
                .ToList();

            var viewModel = new DeTaiDetailViewModel
            {
                DeTai = deTai,
                SoSinhVienDangKyHopLe = soSinhVienDangKyHopLe,
                SoChoCon = soChoCon,
                DanhSachDangKy = danhSachDangKy
            };

            // Lấy danh sách sinh viên để phục vụ tính năng đăng ký thử nghiệm
            ViewBag.SinhViens = await _context.SinhViens
                .AsNoTracking()
                .OrderBy(s => s.HoTen)
                .ToListAsync();

            return View(viewModel);
        }

        // ==============================================================
        // MỤC 6.7 BẢO ĐẢM: KHÔNG CHO ADMIN NHẬP TRỰC TIẾP SoChoCon
        // Form Create & Edit chỉ quản lý SoSinhVienToiDa, không có trường SoChoCon
        // ==============================================================

        // GET: DeTai/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.MaLinhVuc = new SelectList(await _context.LinhVucs.Where(l => l.TrangThai == "HoatDong").ToListAsync(), "MaLinhVuc", "TenLinhVuc");
            return View(new DeTai
            {
                NamHoc = DateTime.Now.Year,
                HocKy = 1,
                SoSinhVienToiDa = 2,
                TrangThai = "MoDangKy"
            });
        }

        // POST: DeTai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDeTai,MaLinhVuc,MoTa,YeuCau,SoSinhVienToiDa,NamHoc,HocKy,TrangThai")] DeTai deTai)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deTai);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới đề tài thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaLinhVuc = new SelectList(await _context.LinhVucs.Where(l => l.TrangThai == "HoatDong").ToListAsync(), "MaLinhVuc", "TenLinhVuc", deTai.MaLinhVuc);
            return View(deTai);
        }

        // GET: DeTai/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var deTai = await _context.DeTais.FindAsync(id);
            if (deTai == null) return NotFound();
            ViewBag.MaLinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "MaLinhVuc", "TenLinhVuc", deTai.MaLinhVuc);
            return View(deTai);
        }

        // POST: DeTai/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDeTai,TenDeTai,MaLinhVuc,MoTa,YeuCau,SoSinhVienToiDa,NamHoc,HocKy,TrangThai")] DeTai deTai)
        {
            if (id != deTai.MaDeTai) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deTai);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật đề tài thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DeTais.Any(e => e.MaDeTai == deTai.MaDeTai)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaLinhVuc = new SelectList(await _context.LinhVucs.ToListAsync(), "MaLinhVuc", "TenLinhVuc", deTai.MaLinhVuc);
            return View(deTai);
        }

        // POST: DeTai/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deTai = await _context.DeTais.FindAsync(id);
            if (deTai != null)
            {
                _context.DeTais.Remove(deTai);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa đề tài thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==============================================================
        // MỤC 6.7 RÀNG BUỘC NGHIỆP VỤ: NẾU SoChoCon = 0 THÌ KHÔNG NHẬN THÊM SV
        // ==============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyDeTai(int maDeTai, string maSinhVien, string? lyDoDangKy)
        {
            var deTai = await _context.DeTais
                .Include(d => d.DangKyDeTais)
                .FirstOrDefaultAsync(d => d.MaDeTai == maDeTai);

            if (deTai == null) return NotFound();

            // Tính số lượng hợp lệ đã duyệt thực tế
            int soDaDuyet = deTai.DangKyDeTais.Count(dk => dk.TrangThai == "DaDuyet");
            int soChoCon = deTai.SoSinhVienToiDa - soDaDuyet;

            // RÀNG BUỘC 6.7: Nếu SoChoCon <= 0, không được nhận thêm sinh viên
            if (soChoCon <= 0)
            {
                TempData["ErrorMessage"] = $"Đề tài đã hết chỗ (Số chỗ còn = {soChoCon} <= 0). Theo quy định mục 6.7, không được nhận thêm sinh viên!";
                return RedirectToAction(nameof(Details), new { id = maDeTai });
            }

            if (deTai.TrangThai != "MoDangKy")
            {
                TempData["ErrorMessage"] = $"Đề tài đang ở trạng thái '{deTai.TrangThai}', không mở nhận đăng ký!";
                return RedirectToAction(nameof(Details), new { id = maDeTai });
            }

            // Kiểm tra trùng sinh viên
            bool daTonTai = deTai.DangKyDeTais.Any(dk => dk.MaSinhVien == maSinhVien && dk.TrangThai != "DaHuy" && dk.TrangThai != "TuChoi");
            if (daTonTai)
            {
                TempData["ErrorMessage"] = "Sinh viên này đã có đăng ký hợp lệ cho đề tài!";
                return RedirectToAction(nameof(Details), new { id = maDeTai });
            }

            var dangKy = new DangKyDeTai
            {
                MaDeTai = maDeTai,
                MaSinhVien = maSinhVien,
                NgayDangKy = DateTime.Now,
                LyDoDangKy = lyDoDangKy,
                TrangThai = "ChoDuyet"
            };

            _context.DangKyDeTais.Add(dangKy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký đề tài thành công! Đang ở trạng thái Chờ duyệt.";
            return RedirectToAction(nameof(Details), new { id = maDeTai });
        }

        // ==============================================================
        // MỤC 6.7: DUYỆT / TỪ CHỐI / HỦY ĐĂNG KÝ ĐỂ KIỂM TOÁN TỨC THÌ SoChoCon
        // ==============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiDangKy(int maDangKy, string trangThaiMoi, string? ghiChu)
        {
            var dangKy = await _context.DangKyDeTais
                .Include(d => d.DeTai)
                    .ThenInclude(dt => dt.DangKyDeTais)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dangKy == null) return NotFound();

            if (trangThaiMoi == "DaDuyet")
            {
                int daDuyetHienTai = dangKy.DeTai.DangKyDeTais.Count(dk => dk.TrangThai == "DaDuyet" && dk.MaDangKy != maDangKy);
                if (daDuyetHienTai >= dangKy.DeTai.SoSinhVienToiDa)
                {
                    TempData["WarningMessage"] = "Cảnh báo: Việc duyệt này đã khiến số sinh viên vượt quá sĩ số tối đa (SoChoCon < 0 - Lỗi nghiệp vụ mục 6.7)!";
                }
                else
                {
                    TempData["SuccessMessage"] = "Đã duyệt đăng ký đề tài thành công!";
                }
            }
            else
            {
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đăng ký thành '{trangThaiMoi}'!";
            }

            dangKy.TrangThai = trangThaiMoi;
            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                dangKy.GhiChuDuyet = ghiChu;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = dangKy.MaDeTai });
        }
    }
}
