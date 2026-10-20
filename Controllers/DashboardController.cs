// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 4 - Dashboard tổng quan (mục 8.7 đề tài)
// Số liệu lấy trực tiếp từ Database bằng LINQ tại thời điểm truy vấn, không lưu tổng số cố định.

using Microsoft.AspNetCore.Mvc;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ThongKeService _thongKe;

        public DashboardController(ThongKeService thongKe)
        {
            _thongKe = thongKe;
        }

        // GET: Trang tổng quan
        public async Task<IActionResult> Index()
        {
            ViewData["SoLieu"] = await _thongKe.LaySoLieuDashboardAsync();
            ViewData["DeTaiTheoLinhVuc"] = await _thongKe.ThongKeDeTaiTheoLinhVucAsync();
            ViewData["DangKyTheoTrangThai"] = await _thongKe.ThongKeDangKyTheoTrangThaiAsync();
            return View();
        }
    }
}