using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLCHNT.Migrations
{
    /// <inheritdoc />
    public partial class updateoder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderEntityId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderEntityId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderEntityId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderEntityId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderEntityId",
                table: "OrderDetails",
                column: "OrderEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderEntityId",
                table: "OrderDetails",
                column: "OrderEntityId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
