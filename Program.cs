using Microsoft.EntityFrameworkCore;
using QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Data;


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

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DeTai}/{action=Index}/{id?}");


app.Run();
