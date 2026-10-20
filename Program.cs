// Họ và tên: Đoàn Quốc Hợp
// MSSV: 23103100097
// Nội dung thực hiện: Module 1 - Tài khoản, đăng nhập, phân quyền và quản lý lĩnh vực đề tài.

using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Services;

// Tự động nhận diện thư mục gốc chứa wwwroot dù chạy trong VS hay click trực tiếp file .exe
var currentDir = Directory.GetCurrentDirectory();
var contentRoot = Directory.Exists(Path.Combine(currentDir, "wwwroot"))
    ? currentDir
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.Exists(contentRoot) ? contentRoot : currentDir
});

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

// Đăng ký lớp nghiệp vụ dùng chung cho Module 3 và Module 4 (mục 12.1 đề tài).
// AddScoped: mỗi request có 1 instance, dùng chung DbContext theo vòng đời request.
builder.Services.AddScoped<NghipVuService>();
builder.Services.AddScoped<ThongKeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Áp dụng các migration để tạo bảng (và mở rộng bảng) nếu database chưa có
        context.Database.Migrate();

        DbSeeder.SeedData(context); // Gọi hàm nạp dữ liệu
    }
    catch (Exception ex)
    {
        // Ghi log nếu có lỗi xảy ra trong quá trình khởi tạo database / nạp Seed Data
        logger.LogError(ex, "Có lỗi xảy ra khi khởi tạo database hoặc nạp Seed Data.");
    }
}

// Khởi tạo và seed dữ liệu mẫu cho database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Có lỗi xảy ra khi khởi tạo cơ sở dữ liệu.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Kích hoạt Session
app.UseSession();

app.UseAuthorization();

// Static files (.NET 9+: MapStaticAssets tối ưu cache, thay thế UseStaticFiles)
app.MapStaticAssets();

// Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DeTai}/{action=Index}/{id?}");

app.Run();
