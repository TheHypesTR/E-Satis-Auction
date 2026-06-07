using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESatisAuction.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCartPaymentCommerceOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppliedCouponCampaignId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppliedFreeShippingCampaignId",
                table: "PurchaseOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "PurchaseOrders",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingAmount",
                table: "PurchaseOrders",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "AppliedCouponCampaignId",
                table: "PurchaseOrderLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CouponDiscountAmount",
                table: "PurchaseOrderLines",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "PurchaseOrderLines",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalUnitPrice",
                table: "PurchaseOrderLines",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalAmount",
                table: "PurchaseOrderLines",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Campaigns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Campaigns",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumOrderAmount",
                table: "Campaigns",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductListingId",
                table: "Campaigns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "Campaigns",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE "PurchaseOrderLines"
                SET
                    "FinalUnitPrice" = "DiscountedUnitPrice",
                    "SubtotalAmount" = "UnitPrice" * "Quantity",
                    "DiscountAmount" = ("UnitPrice" - "DiscountedUnitPrice") * "Quantity"
                """);

            migrationBuilder.CreateTable(
                name: "PartSaleOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedPartItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitOfMeasure = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartSaleOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartSaleOperations_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartSaleOperations_Items_CreatedPartItemId",
                        column: x => x.CreatedPartItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartSaleOperations_Items_SourceItemId",
                        column: x => x.SourceItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartSaleOperations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ProductListingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    AppliedCouponCampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreviewSubtotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviewDiscountAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviewShippingAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviewTotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_Campaigns_AppliedCouponCampaignId",
                        column: x => x.AppliedCouponCampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_ProductListings_ProductListingId",
                        column: x => x.ProductListingId,
                        principalTable: "ProductListings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSaleRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEstimatedValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AcquisitionPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TargetResalePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ExpectedProfit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AdminNote = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSaleRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSaleRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSaleRequests_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_IdempotencyKey",
                table: "PurchaseOrders",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SourceItemId",
                table: "Items",
                column: "SourceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CategoryId",
                table: "Campaigns",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CouponCode",
                table: "Campaigns",
                column: "CouponCode",
                unique: true,
                filter: "\"CouponCode\" IS NOT NULL AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_ProductListingId",
                table: "Campaigns",
                column: "ProductListingId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleOperations_CreatedAt",
                table: "PartSaleOperations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleOperations_CreatedPartItemId",
                table: "PartSaleOperations",
                column: "CreatedPartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleOperations_FacilityId",
                table: "PartSaleOperations",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleOperations_ProductId",
                table: "PartSaleOperations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PartSaleOperations_SourceItemId",
                table: "PartSaleOperations",
                column: "SourceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_IdempotencyKey",
                table: "PaymentAttempts",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_PurchaseOrderId",
                table: "PaymentAttempts",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_Status_ExpiresAt",
                table: "PaymentAttempts",
                columns: new[] { "Status", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_UserId",
                table: "PaymentAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_AppliedCouponCampaignId",
                table: "ShoppingCarts",
                column: "AppliedCouponCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProductListingId",
                table: "ShoppingCarts",
                column: "ProductListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                unique: true,
                filter: "\"Status\" = 1 AND \"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaleRequests_CategoryId",
                table: "UserSaleRequests",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaleRequests_CreatedAt",
                table: "UserSaleRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaleRequests_Status",
                table: "UserSaleRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaleRequests_UserId",
                table: "UserSaleRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Items_SourceItemId",
                table: "Items",
                column: "SourceItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Items_SourceItemId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "PartSaleOperations");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "UserSaleRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_IdempotencyKey",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Items_SourceItemId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CategoryId",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CouponCode",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_ProductListingId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "AppliedCouponCampaignId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "AppliedFreeShippingCampaignId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ShippingAmount",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "AppliedCouponCampaignId",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponDiscountAmount",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "FinalUnitPrice",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "SubtotalAmount",
                table: "PurchaseOrderLines");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "MinimumOrderAmount",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "ProductListingId",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Campaigns");
        }
    }
}
