using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyAdministrator.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrencyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "DECIMAL(10,6)", precision: 10, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyId", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Country",
                table: "Currencies",
                column: "Country",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
