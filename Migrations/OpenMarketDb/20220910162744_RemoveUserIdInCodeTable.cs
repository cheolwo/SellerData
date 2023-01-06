using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerData.Migrations.OpenMarketDb
{
    public partial class RemoveUserIdInCodeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OpenMarketCommonCategory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommodityOriginCode");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommodityDeliveryCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "OpenMarketCommonCategory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CommodityOriginCode",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CommodityDeliveryCode",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
