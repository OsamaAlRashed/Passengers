using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Areas_AreaId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Address_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Products_ProductId",
                table: "Discount");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_AspNetUsers_CustomerId",
                table: "Rate");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_Orders_OrderId",
                table: "Rate");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_Products_ProductId",
                table: "Rate");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopContact_AspNetUsers_ShopId",
                table: "ShopContact");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopSchedule_AspNetUsers_ShopId",
                table: "ShopSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopSchedule",
                table: "ShopSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopContact",
                table: "ShopContact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rate",
                table: "Rate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Discount",
                table: "Discount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "ShopSchedule",
                newName: "ShopSchedules");

            migrationBuilder.RenameTable(
                name: "ShopContact",
                newName: "ShopContacts");

            migrationBuilder.RenameTable(
                name: "Rate",
                newName: "Rates");

            migrationBuilder.RenameTable(
                name: "Discount",
                newName: "Discounts");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_ShopSchedule_ShopId",
                table: "ShopSchedules",
                newName: "IX_ShopSchedules_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopSchedule_DateCreated",
                table: "ShopSchedules",
                newName: "IX_ShopSchedules_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_ShopContact_ShopId",
                table: "ShopContacts",
                newName: "IX_ShopContacts_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopContact_DateCreated",
                table: "ShopContacts",
                newName: "IX_ShopContacts_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Rate_ProductId",
                table: "Rates",
                newName: "IX_Rates_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Rate_OrderId",
                table: "Rates",
                newName: "IX_Rates_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Rate_DateCreated",
                table: "Rates",
                newName: "IX_Rates_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Rate_CustomerId",
                table: "Rates",
                newName: "IX_Rates_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Discount_ProductId",
                table: "Discounts",
                newName: "IX_Discounts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Discount_DateCreated",
                table: "Discounts",
                newName: "IX_Discounts_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Address_DateCreated",
                table: "Addresses",
                newName: "IX_Addresses_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Address_AreaId",
                table: "Addresses",
                newName: "IX_Addresses_AreaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopSchedules",
                table: "ShopSchedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopContacts",
                table: "ShopContacts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rates",
                table: "Rates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Discounts",
                table: "Discounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactUs_DateCreated",
                table: "ContactUs",
                column: "DateCreated");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DateCreated",
                table: "Notifications",
                column: "DateCreated");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Areas_AreaId",
                table: "Addresses",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Addresses_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Products_ProductId",
                table: "Discounts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_AspNetUsers_CustomerId",
                table: "Rates",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Orders_OrderId",
                table: "Rates",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Products_ProductId",
                table: "Rates",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopContacts_AspNetUsers_ShopId",
                table: "ShopContacts",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopSchedules_AspNetUsers_ShopId",
                table: "ShopSchedules",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Areas_AreaId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Addresses_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Products_ProductId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rates_AspNetUsers_CustomerId",
                table: "Rates");

            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Orders_OrderId",
                table: "Rates");

            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Products_ProductId",
                table: "Rates");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopContacts_AspNetUsers_ShopId",
                table: "ShopContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopSchedules_AspNetUsers_ShopId",
                table: "ShopSchedules");

            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopSchedules",
                table: "ShopSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopContacts",
                table: "ShopContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rates",
                table: "Rates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Discounts",
                table: "Discounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "ShopSchedules",
                newName: "ShopSchedule");

            migrationBuilder.RenameTable(
                name: "ShopContacts",
                newName: "ShopContact");

            migrationBuilder.RenameTable(
                name: "Rates",
                newName: "Rate");

            migrationBuilder.RenameTable(
                name: "Discounts",
                newName: "Discount");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_ShopSchedules_ShopId",
                table: "ShopSchedule",
                newName: "IX_ShopSchedule_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopSchedules_DateCreated",
                table: "ShopSchedule",
                newName: "IX_ShopSchedule_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_ShopContacts_ShopId",
                table: "ShopContact",
                newName: "IX_ShopContact_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopContacts_DateCreated",
                table: "ShopContact",
                newName: "IX_ShopContact_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_ProductId",
                table: "Rate",
                newName: "IX_Rate_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_OrderId",
                table: "Rate",
                newName: "IX_Rate_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_DateCreated",
                table: "Rate",
                newName: "IX_Rate_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Rates_CustomerId",
                table: "Rate",
                newName: "IX_Rate_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Discounts_ProductId",
                table: "Discount",
                newName: "IX_Discount_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Discounts_DateCreated",
                table: "Discount",
                newName: "IX_Discount_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_DateCreated",
                table: "Address",
                newName: "IX_Address_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_AreaId",
                table: "Address",
                newName: "IX_Address_AreaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopSchedule",
                table: "ShopSchedule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopContact",
                table: "ShopContact",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rate",
                table: "Rate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Discount",
                table: "Discount",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Areas_AreaId",
                table: "Address",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Address_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Products_ProductId",
                table: "Discount",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_AspNetUsers_CustomerId",
                table: "Rate",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_Orders_OrderId",
                table: "Rate",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_Products_ProductId",
                table: "Rate",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopContact_AspNetUsers_ShopId",
                table: "ShopContact",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopSchedule_AspNetUsers_ShopId",
                table: "ShopSchedule",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
