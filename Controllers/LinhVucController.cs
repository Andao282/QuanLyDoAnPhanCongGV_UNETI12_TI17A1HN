// Họ và tên: Đoàn Quốc Hợp
// MSSV: 23103100097
// Nội dung thực hiện: Module 1 - Tài khoản, đăng nhập, phân quyền và quản lý lĩnh vực đề tài.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class LinhVucController : Controller
    {
        private readonly AppDbContext _context;

        public LinhVucController(AppDbContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("VaiTro") == "Admin";
        }

        // =========================
        // DANH SÁCH LĨNH VỰC
        // =========================

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            var danhSach = await _context.LinhVucs
                .AsNoTracking()
                .ToListAsync();

            return View(danhSach);
        }

        // =========================
        // CHI TIẾT
        // =========================

        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var linhVuc = await _context.LinhVucs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaLinhVuc == id);

            if (linhVuc == null)
            {
                return NotFound();
            }

            return View(linhVuc);
        }

        // =========================
        // CREATE - GET
        // =========================

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            return View();
        }

        // =========================
        // CREATE - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LinhVuc linhVuc)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            linhVuc.TenLinhVuc = linhVuc.TenLinhVuc?.Trim();

            // Kiểm tra tên lĩnh vực không được trống
            if (string.IsNullOrWhiteSpace(linhVuc.TenLinhVuc))
            {
                ModelState.AddModelError(
                    "TenLinhVuc",
                    "Tên lĩnh vực không được để trống.");
            }

            // Kiểm tra tên lĩnh vực bị trùng
            if (!string.IsNullOrWhiteSpace(linhVuc.TenLinhVuc))
            {
                bool trungTen = await _context.LinhVucs
                    .AnyAsync(x =>
                        x.TenLinhVuc.ToLower()
                        == linhVuc.TenLinhVuc.ToLower());

                if (trungTen)
                {
                    ModelState.AddModelError(
                        "TenLinhVuc",
                        "Tên lĩnh vực đã tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(linhVuc);
            }

            await _context.LinhVucs.AddAsync(linhVuc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm lĩnh vực thành công.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var linhVuc = await _context.LinhVucs
                .FindAsync(id);

            if (linhVuc == null)
            {
                return NotFound();
            }

            return View(linhVuc);
        }

        // =========================
        // EDIT - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            LinhVuc linhVuc)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            if (id != linhVuc.MaLinhVuc)
            {
                return NotFound();
            }

            linhVuc.TenLinhVuc = linhVuc.TenLinhVuc?.Trim();

            // Kiểm tra tên không được trống
            if (string.IsNullOrWhiteSpace(linhVuc.TenLinhVuc))
            {
                ModelState.AddModelError(
                    "TenLinhVuc",
                    "Tên lĩnh vực không được để trống.");
            }

            // Kiểm tra trùng tên với lĩnh vực khác
            if (!string.IsNullOrWhiteSpace(linhVuc.TenLinhVuc))
            {
                bool trungTen = await _context.LinhVucs
                    .AnyAsync(x =>
                        x.MaLinhVuc != id &&
                        x.TenLinhVuc.ToLower()
                        == linhVuc.TenLinhVuc.ToLower());

                if (trungTen)
                {
                    ModelState.AddModelError(
                        "TenLinhVuc",
                        "Tên lĩnh vực đã tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(linhVuc);
            }

            try
            {
                _context.Update(linhVuc);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật lĩnh vực thành công.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LinhVucs
                    .AnyAsync(x => x.MaLinhVuc == id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var linhVuc = await _context.LinhVucs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaLinhVuc == id);

            if (linhVuc == null)
            {
                return NotFound();
            }

            return View(linhVuc);
        }

        // =========================
        // DELETE - POST
        // =========================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("DangNhap", "Home");
            }

            // Kiểm tra lĩnh vực đang được Giảng viên sử dụng
            bool coGiangVien = await _context.GiangViens
                .AnyAsync(x => x.MaLinhVuc == id);

            // Kiểm tra lĩnh vực đang được Đề tài sử dụng
            bool coDeTai = await _context.DeTais
                .AnyAsync(x => x.MaLinhVuc == id);

            if (coGiangVien || coDeTai)
            {
                TempData["Error"] =
                    "Không thể xóa lĩnh vực vì đang có dữ liệu liên quan.";

                return RedirectToAction(nameof(Index));
            }

            var linhVuc = await _context.LinhVucs
                .FindAsync(id);

            if (linhVuc == null)
            {
                return NotFound();
            }

            _context.LinhVucs.Remove(linhVuc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa lĩnh vực thành công.";

            return RedirectToAction(nameof(Index));
        }
    }
}