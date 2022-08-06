using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class OrderDriverTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDriver_AspNetUsers_DriverId",
                table: "OrderDriver");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDriver_Orders_OrderId",
                table: "OrderDriver");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDriver",
                table: "OrderDriver");

            migrationBuilder.RenameTable(
                name: "OrderDriver",
                newName: "OrderDrivers");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDriver_OrderId",
                table: "OrderDrivers",
                newName: "IX_OrderDrivers_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDriver_DriverId",
                table: "OrderDrivers",
                newName: "IX_OrderDrivers_DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDriver_DateCreated",
                table: "OrderDrivers",
                newName: "IX_OrderDrivers_DateCreated");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDrivers",
                table: "OrderDrivers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDrivers_AspNetUsers_DriverId",
                table: "OrderDrivers",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDrivers_Orders_OrderId",
                table: "OrderDrivers",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDrivers_AspNetUsers_DriverId",
                table: "OrderDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDrivers_Orders_OrderId",
                table: "OrderDrivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderDrivers",
                table: "OrderDrivers");

            migrationBuilder.RenameTable(
                name: "OrderDrivers",
                newName: "OrderDriver");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDrivers_OrderId",
                table: "OrderDriver",
                newName: "IX_OrderDriver_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDrivers_DriverId",
                table: "OrderDriver",
                newName: "IX_OrderDriver_DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDrivers_DateCreated",
                table: "OrderDriver",
                newName: "IX_OrderDriver_DateCreated");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderDriver",
                table: "OrderDriver",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDriver_AspNetUsers_DriverId",
                table: "OrderDriver",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDriver_Orders_OrderId",
                table: "OrderDriver",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
