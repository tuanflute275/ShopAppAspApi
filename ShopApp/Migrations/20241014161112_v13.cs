using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Migrations
{
    /// <inheritdoc />
    public partial class v13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "BlogComment",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "BlogComment",
                newName: "Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "BlogComment",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "BlogComment",
                newName: "message");
        }
    }
}
