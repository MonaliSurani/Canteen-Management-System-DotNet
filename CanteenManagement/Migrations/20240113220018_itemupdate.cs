using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenManagement.Migrations
{
    /// <inheritdoc />
    public partial class itemupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailbleItems",
                table: "Item",
                newName: "AvailableItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableItems",
                table: "Item",
                newName: "AvailbleItems");
        }
    }
}
