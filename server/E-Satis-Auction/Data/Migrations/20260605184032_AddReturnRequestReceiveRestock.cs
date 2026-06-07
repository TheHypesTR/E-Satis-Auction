using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESatisAuction.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnRequestReceiveRestock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiveNote",
                table: "ReturnRequests",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedAt",
                table: "ReturnRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedByUserId",
                table: "ReturnRequests",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiveNote",
                table: "ReturnRequestLines",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedQuantity",
                table: "ReturnRequestLines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RestockedQuantity",
                table: "ReturnRequestLines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReceivedAt",
                table: "ReturnRequests",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReceivedByUserId",
                table: "ReturnRequests",
                column: "ReceivedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_AspNetUsers_ReceivedByUserId",
                table: "ReturnRequests",
                column: "ReceivedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_AspNetUsers_ReceivedByUserId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_ReceivedAt",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_ReceivedByUserId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ReceiveNote",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ReceivedAt",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ReceivedByUserId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ReceiveNote",
                table: "ReturnRequestLines");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                table: "ReturnRequestLines");

            migrationBuilder.DropColumn(
                name: "RestockedQuantity",
                table: "ReturnRequestLines");
        }
    }
}
