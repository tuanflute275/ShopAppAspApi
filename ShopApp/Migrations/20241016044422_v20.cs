using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Migrations
{
    /// <inheritdoc />
    public partial class v20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Coupon_CouponId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CouponId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Order");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_CouponId",
                table: "Order",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Coupon_CouponId",
                table: "Order",
                column: "CouponId",
                principalTable: "Coupon",
                principalColumn: "CouponId");
        }
    }
}
