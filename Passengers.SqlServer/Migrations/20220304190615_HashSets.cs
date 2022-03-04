using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class HashSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_UserId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_UserId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "Shop",
                table: "Offers",
                newName: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ShopId",
                table: "Offers",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_ShopId",
                table: "Offers",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_ShopId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_ShopId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "ShopId",
                table: "Offers",
                newName: "Shop");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Offers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_UserId",
                table: "Offers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_UserId",
                table: "Offers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
