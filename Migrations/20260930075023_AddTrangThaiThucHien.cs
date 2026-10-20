using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAnPhanCongGV_UNETI12_TI17A1HN.Migrations
{
    /// <inheritdoc />
    public partial class AddTrangThaiThucHien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThaiThucHien",
                table: "DangKyDeTais",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                // Các đăng ký đã có sẵn trong Database được coi là chưa bắt đầu thực hiện
                defaultValue: "ChuaBatDau");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiThucHien",
                table: "DangKyDeTais");
        }
    }
}
