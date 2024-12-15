using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreSystem.Migrations
{
    /// <inheritdoc />
    public partial class Bonus_System_Adding_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Bonus",
                table: "Employees",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bonus",
                table: "Employees");
        }
    }
}
