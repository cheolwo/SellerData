using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerData.Migrations.OpenMarketDb
{
    public partial class UpdateOpenMarketDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessKey",
                table: "OpenMarket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "OpenMarket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "OpenMarket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessKey",
                table: "OpenMarket");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "OpenMarket");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "OpenMarket");
        }
    }
}
