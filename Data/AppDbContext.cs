// Họ và tên: Đoàn Quốc Hợp
// MSSV: 23103100097
// Nội dung thực hiện: Module 1 - Tài khoản, đăng nhập, phân quyền và quản lý lĩnh vực đề tài.

using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        public DbSet<LinhVuc> LinhVucs { get; set; }

        public DbSet<GiangVien> GiangViens { get; set; }

        public DbSet<DeTai> DeTais { get; set; }

        public DbSet<SinhVien> SinhViens { get; set; }

        public DbSet<DangKyDeTai> DangKyDeTais { get; set; }

        public DbSet<PhanCongHuongDan> PhanCongHuongDans { get; set; }
    }
}