using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class fixAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryShopStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShopOrderType",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "OrderStatus",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryShopStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShopOrderType",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
