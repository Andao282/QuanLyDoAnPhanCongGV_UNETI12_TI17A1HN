using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Models;

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo 7 bảng bắt buộc của đồ án
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<LinhVuc> LinhVucs { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<DeTai> DeTais { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<DangKyDeTai> DangKyDeTais { get; set; }
        public DbSet<PhanCongHuongDan> PhanCongHuongDans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Bạn (hoặc các thành viên) sẽ viết Fluent API cấu hình quan hệ (1-1, 1-n) ở đây nếu cần thiết
            modelBuilder.Entity<PhanCongHuongDan>()
        .HasOne(p => p.GiangVien)
        .WithMany(g => g.PhanCongHuongDans)
        .HasForeignKey(p => p.MaGiangVien)
        .OnDelete(DeleteBehavior.NoAction);

            // Ngắt xóa dây chuyền từ Đề tài đến Đăng ký đề tài (phòng ngừa thêm)
            modelBuilder.Entity<DangKyDeTai>()
                .HasOne(d => d.DeTai)
                .WithMany(dt => dt.DangKyDeTais)
                .HasForeignKey(d => d.MaDeTai)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}