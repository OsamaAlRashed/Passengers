using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Passengers.SqlServer.Migrations
{
    public partial class EasyRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_DateCreated",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "DateUpdated",
                table: "RefreshTokens",
                newName: "ExpiredDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiredDate",
                table: "RefreshTokens",
                newName: "DateUpdated");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DateCreated",
                table: "RefreshTokens",
                column: "DateCreated");
        }
    }
}
