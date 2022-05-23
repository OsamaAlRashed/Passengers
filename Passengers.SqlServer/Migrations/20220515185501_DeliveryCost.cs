using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class DeliveryCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAmount",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "TotalPrice",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
