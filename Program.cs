// Họ và tên: Đoàn Quốc Hợp
// MSSV: 23103100097
// Nội dung thực hiện: Module 1 - Tài khoản, đăng nhập, phân quyền và quản lý lĩnh vực đề tài.

using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký MVC
builder.Services.AddControllersWithViews();

// Cấu hình Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình DbContext kết nối SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Cấu hình môi trường
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Kích hoạt Session
app.UseSession();

app.UseAuthorization();

// Static files
app.MapStaticAssets();

// Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();