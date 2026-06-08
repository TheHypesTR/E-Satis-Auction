using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESatisAuction.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    StartingPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumBidIncrement = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalEndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentWinningBidId = table.Column<Guid>(type: "uuid", nullable: true),
                    WinningUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    WinningBidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentAttemptId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    WaitingFeeAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ServiceFeeAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SellerPayoutAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformRevenueAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_AspNetUsers_SellerUserId",
                        column: x => x.SellerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auctions_PaymentAttempts_PaymentAttemptId",
                        column: x => x.PaymentAttemptId,
                        principalTable: "PaymentAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auctions_ProductListings_ProductListingId",
                        column: x => x.ProductListingId,
                        principalTable: "ProductListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auctions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auctions_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuctionBids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BidderUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionBids_AspNetUsers_BidderUserId",
                        column: x => x.BidderUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionBids_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuctionInventoryReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionInventoryReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionInventoryReservations_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuctionInventoryReservations_Items_OriginalItemId",
                        column: x => x.OriginalItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionInventoryReservations_Items_ReservedItemId",
                        column: x => x.ReservedItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBids_AuctionId",
                table: "AuctionBids",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBids_AuctionId_BidderUserId_IdempotencyKey",
                table: "AuctionBids",
                columns: new[] { "AuctionId", "BidderUserId", "IdempotencyKey" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionBids_BidderUserId",
                table: "AuctionBids",
                column: "BidderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionInventoryReservations_AuctionId",
                table: "AuctionInventoryReservations",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionInventoryReservations_OriginalItemId",
                table: "AuctionInventoryReservations",
                column: "OriginalItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionInventoryReservations_ReservedItemId",
                table: "AuctionInventoryReservations",
                column: "ReservedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionInventoryReservations_Status",
                table: "AuctionInventoryReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_EndsAt",
                table: "Auctions",
                column: "EndsAt");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_PaymentAttemptId",
                table: "Auctions",
                column: "PaymentAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ProductId",
                table: "Auctions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ProductListingId",
                table: "Auctions",
                column: "ProductListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_PurchaseOrderId",
                table: "Auctions",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_SellerUserId",
                table: "Auctions",
                column: "SellerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_StartsAt",
                table: "Auctions",
                column: "StartsAt");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_Status",
                table: "Auctions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_WinningUserId",
                table: "Auctions",
                column: "WinningUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionBids");

            migrationBuilder.DropTable(
                name: "AuctionInventoryReservations");

            migrationBuilder.DropTable(
                name: "Auctions");
        }
    }
}
