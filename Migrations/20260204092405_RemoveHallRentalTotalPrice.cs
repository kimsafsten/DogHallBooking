using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HallBooking.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHallRentalTotalPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_price",
                table: "hall_rentals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "total_price",
                table: "hall_rentals",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
