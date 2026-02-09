using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HallBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "courses",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "courses");
        }
    }
}
