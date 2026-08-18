using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstatePortal.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsedListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedListings",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsedListings",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
