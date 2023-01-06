using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerData.Migrations.OpenMarketDb
{
    public partial class InitCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenMarketCommonCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Category1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenMarketCommonCategory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenMarketCommonCategory");
        }
    }
}
