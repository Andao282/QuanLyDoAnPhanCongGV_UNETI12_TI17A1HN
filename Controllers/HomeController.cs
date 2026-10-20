// Họ và tên: Đoàn Quốc Hợp
// MSSV: 23103100097
// Nội dung thực hiện: Module 1 - Tài khoản, đăng nhập, phân quyền.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using System.Diagnostics;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult DangNhap()
        {
            // Nếu đã đăng nhập thì không cho quay lại trang đăng nhập
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(
            string tenDangNhap,
            string matKhau)
        {
            // Kiểm tra tên đăng nhập
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                ViewBag.Error = "Tên đăng nhập không được để trống.";
                return View();
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Mật khẩu không được để trống.";
                return View();
            }

            // Tìm tài khoản trong database
            var taiKhoan = await _context.TaiKhoans
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.TenDangNhap == tenDangNhap &&
                    x.MatKhau == matKhau);

            // Không tìm thấy tài khoản
            if (taiKhoan == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            // Kiểm tra trạng thái tài khoản
            if (taiKhoan.TrangThai != "HoatDong")
            {
                ViewBag.Error = "Tài khoản đã bị khóa.";
                return View();
            }

            // Lưu thông tin tài khoản vào Session
            HttpContext.Session.SetInt32(
                "MaTaiKhoan",
                taiKhoan.MaTaiKhoan);

            HttpContext.Session.SetString(
                "HoTen",
                taiKhoan.HoTen);

            HttpContext.Session.SetString(
                "VaiTro",
                taiKhoan.VaiTro);

            // Nếu là Admin -> vào quản lý lĩnh vực
            if (taiKhoan.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "LinhVuc");
            }

            // Nếu không phải Admin -> về trang chủ
            return RedirectToAction("Index", "Home");
        }

        // Đăng xuất
        [HttpGet]
        public IActionResult DangXuat()
        {
            // Xóa toàn bộ Session
            HttpContext.Session.Clear();

            // Quay lại trang đăng nhập
            return RedirectToAction("DangNhap");
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}