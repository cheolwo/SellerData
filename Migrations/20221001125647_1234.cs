using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerData.Migrations
{
    public partial class _1234 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name : "FilterCategoryCodes",
                table: "SellerMarket",
                nullable : true
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
