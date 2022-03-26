using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class FavTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_AspNetUsers_CustomerId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_AspNetUsers_ShopId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Products_ProductId",
                table: "Favorite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorite",
                table: "Favorite");

            migrationBuilder.RenameTable(
                name: "Favorite",
                newName: "Favorites");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_ShopId",
                table: "Favorites",
                newName: "IX_Favorites_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_ProductId",
                table: "Favorites",
                newName: "IX_Favorites_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_DateCreated",
                table: "Favorites",
                newName: "IX_Favorites_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_CustomerId",
                table: "Favorites",
                newName: "IX_Favorites_CustomerId");

            migrationBuilder.AddColumn<int>(
                name: "ShopOrderType",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_CustomerId",
                table: "Favorites",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_ShopId",
                table: "Favorites",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Products_ProductId",
                table: "Favorites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_CustomerId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_ShopId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Products_ProductId",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "ShopOrderType",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Favorites",
                newName: "Favorite");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_ShopId",
                table: "Favorite",
                newName: "IX_Favorite_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_ProductId",
                table: "Favorite",
                newName: "IX_Favorite_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_DateCreated",
                table: "Favorite",
                newName: "IX_Favorite_DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_CustomerId",
                table: "Favorite",
                newName: "IX_Favorite_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorite",
                table: "Favorite",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_AspNetUsers_CustomerId",
                table: "Favorite",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_AspNetUsers_ShopId",
                table: "Favorite",
                column: "ShopId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Products_ProductId",
                table: "Favorite",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
