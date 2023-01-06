using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerData.Migrations
{
    public partial class InitNullable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerEMCommodity_SellerMarket_SellerMarketId",
                table: "SellerEMCommodity");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerEMCommodity_SellerMCommodity_SellerMCommodityId",
                table: "SellerEMCommodity");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerEMCommodity_SellerMMCommodity_SellerMMCommodityId",
                table: "SellerEMCommodity");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerMCommodity_SellerMarket_SellerMarketId",
                table: "SellerMCommodity");

            migrationBuilder.AlterColumn<string>(
                name: "SellerMarketId",
                table: "SellerMCommodity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SellerMarketId",
                table: "SellerEMCommodity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SellerMMCommodityId",
                table: "SellerEMCommodity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SellerMCommodityId",
                table: "SellerEMCommodity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerEMCommodity_SellerMarket_SellerMarketId",
                table: "SellerEMCommodity",
                column: "SellerMarketId",
                principalTable: "SellerMarket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerEMCommodity_SellerMCommodity_SellerMCommodityId",
                table: "SellerEMCommodity",
                column: "SellerMCommodityId",
                principalTable: "SellerMCommodity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerEMCommodity_SellerMMCommodity_SellerMMCommodityId",
                table: "SellerEMCommodity",
                column: "SellerMMCommodityId",
                principalTable: "SellerMMCommodity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerMCommodity_SellerMarket_SellerMarketId",
                table: "SellerMCommodity",
                column: "SellerMarketId",
                principalTable: "SellerMarket",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerEMCommodity_SellerMarket_SellerMarketId",
                table: "SellerEMCommodity");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerEMCommodity_SellerMCommodity_SellerMCommodityId",
                table: "SellerEMCommodity");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerEMCommodity_SellerMMCommodity_SellerMMCommodityId",
                table: "SellerEMCommodity");

            migrationBuilder.DropForeignKey(
                name: "FK_SellerMCommodity_SellerMarket_SellerMarketId",
                table: "SellerMCommodity");

            migrationBuilder.AlterColumn<string>(
                name: "SellerMarketId",
                table: "SellerMCommodity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SellerMarketId",
                table: "SellerEMCommodity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SellerMMCommodityId",
                table: "SellerEMCommodity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SellerMCommodityId",
                table: "SellerEMCommodity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerEMCommodity_SellerMarket_SellerMarketId",
                table: "SellerEMCommodity",
                column: "SellerMarketId",
                principalTable: "SellerMarket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerEMCommodity_SellerMCommodity_SellerMCommodityId",
                table: "SellerEMCommodity",
                column: "SellerMCommodityId",
                principalTable: "SellerMCommodity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerEMCommodity_SellerMMCommodity_SellerMMCommodityId",
                table: "SellerEMCommodity",
                column: "SellerMMCommodityId",
                principalTable: "SellerMMCommodity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellerMCommodity_SellerMarket_SellerMarketId",
                table: "SellerMCommodity",
                column: "SellerMarketId",
                principalTable: "SellerMarket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
