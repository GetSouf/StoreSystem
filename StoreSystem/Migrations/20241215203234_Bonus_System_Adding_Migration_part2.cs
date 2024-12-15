using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreSystem.Migrations
{
    /// <inheritdoc />
    public partial class Bonus_System_Adding_Migration_part2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BonusPercentage",
                table: "Products",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusPercentage",
                table: "Products");
        }
    }
}
