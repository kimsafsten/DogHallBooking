using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HallBooking.Migrations
{
    /// <inheritdoc />
    public partial class SeedMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "members",
                columns: new[] { "id", "email", "name" },
                values: new object[,]
                {
                    { 1, "anna@mail.com", "Anna Andersson" },
                    { 2, "brotherbear@mail.com", "Björn Berg" },
                    { 3, "cissi@example.com", "Cecilia Carlsson" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "members",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "members",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "members",
                keyColumn: "id",
                keyValue: 3);
        }
    }
}
