using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Migrations
{
    /// <inheritdoc />
    public partial class v21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_Order_OrderId",
                table: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_OrderId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Coupon");

            migrationBuilder.CreateTable(
                name: "CouponOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponOrders_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "CouponId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CouponOrders_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouponOrders_CouponId",
                table: "CouponOrders",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponOrders_OrderId",
                table: "CouponOrders",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouponOrders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Coupon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_OrderId",
                table: "Coupon",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_Order_OrderId",
                table: "Coupon",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
