// Họ và tên: Đào Hoàng An
// Mã sinh viên: [23103100074]
// Nội dung thực hiện: Module 4 - Màn hình thống kê (mục 8.8 đề tài)
// Mọi số liệu truy vấn bằng LINQ tại thời điểm truy vấn, không nhập hoặc lưu tổng số cố định.

using Microsoft.AspNetCore.Mvc;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ThongKeService _thongKe;

        public ThongKeController(ThongKeService thongKe)
        {
            _thongKe = thongKe;
        }

        // GET: Báo cáo thống kê toàn bộ số liệu theo mục 8.8
        public async Task<IActionResult> Index()
        {
            ViewData["DeTaiTheoLinhVuc"] = await _thongKe.ThongKeDeTaiTheoLinhVucAsync();
            ViewData["DangKyTheoTrangThai"] = await _thongKe.ThongKeDangKyTheoTrangThaiAsync();
            ViewData["ChiTietDeTai"] = await _thongKe.ThongKeSoChoConTungDeTaiAsync();
            ViewData["TaiGiangVien"] = await _thongKe.ThongKeTaiGiangVienAsync();
            ViewData["GvNhieuNhat"] = await _thongKe.TimGiangVienHuongDanNhieuNhatAsync();
            ViewData["DangKyTheoThang"] = await _thongKe.ThongKeDangKyTheoThangAsync();
            ViewData["DeTaiTheoHocKy"] = await _thongKe.ThongKeDeTaiTheoHocKyAsync();

            return View();
        }
    }
}