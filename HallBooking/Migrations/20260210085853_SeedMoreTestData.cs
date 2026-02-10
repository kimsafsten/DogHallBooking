using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HallBooking.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createdAt = new DateTime(2026, 02, 05, 10, 00, 00, DateTimeKind.Unspecified);

            // --------------------
            // COURSES
            // --------------------
            migrationBuilder.InsertData(
                table: "courses",
                columns: new[]
                {
                    "id",
                    "title",
                    "instructor_name",
                    "description",
                    "capacity",
                    "price_per_participant"
                },
                values: new object[,]
                {
                    {
                        10,
                        "Agility Grund",
                        "Sofia Sjöberg",
                        "Introduktion till agility för nybörjare.",
                        8,
                        1495m
                    },
                    {
                        11,
                        "Nosework Nybörjare",
                        "Johan Nilsson",
                        "Grunder i nosework: doft, sök och belöning.",
                        10,
                        1295m
                    }
                });


            // --------------------
            // COURSE SESSIONS
            // --------------------
            migrationBuilder.InsertData(
                table: "course_sessions",
                columns: new[]
                {
                    "id",
                    "course_id",
                    "start_time",
                    "end_time"
                },
                values: new object[,]
                {
                    {
                        100,
                        10,
                        new DateTime(2026, 02, 12, 17, 00, 00, DateTimeKind.Unspecified),
                        new DateTime(2026, 02, 12, 19, 00, 00, DateTimeKind.Unspecified)
                    },
                    {
                        101,
                        10,
                        new DateTime(2026, 02, 19, 17, 00, 00, DateTimeKind.Unspecified),
                        new DateTime(2026, 02, 19, 19, 00, 00, DateTimeKind.Unspecified)
                    },
                    {
                        110,
                        11,
                        new DateTime(2026, 02, 14, 18, 00, 00, DateTimeKind.Unspecified),
                        new DateTime(2026, 02, 14, 20, 00, 00, DateTimeKind.Unspecified)
                    }
                });

            // --------------------
            // COURSE ENROLLMENTS
            // --------------------
            migrationBuilder.InsertData(
                table: "course_enrollments",
                columns: new[]
                {
                    "id",
                    "course_id",
                    "member_id"
                },
                values: new object[,]
                {
                    { 200, 10, 1 },
                    { 201, 10, 2 },
                    { 202, 11, 3 }
                });

            // --------------------
            // HALL RENTAL
            // --------------------
            var rentalStart = new DateTime(2026, 02, 15, 12, 00, 00, DateTimeKind.Unspecified);
            var rentalEnd = new DateTime(2026, 02, 15, 14, 00, 00, DateTimeKind.Unspecified);
            var hourlyPrice = 280m;

            migrationBuilder.InsertData(
                table: "hall_rentals",
                columns: new[]
                {
                    "id",
                    "member_id",
                    "start_time",
                    "end_time",
                    "status",
                    "hourly_price"
                },
                values: new object[]
                {
                    300,
                    1,
                    rentalStart,
                    rentalEnd,
                    0,
                    hourlyPrice
                });

            // --------------------
            // PAYMENT (for hall rental)
            // --------------------
            var hours = 2;
            var totalPrice = hours * hourlyPrice;

            migrationBuilder.InsertData(
                table: "payments",
                columns: new[]
                {
                    "id",
                    "member_id",
                    "amount",
                    "status",
                    "hall_rental_id",
                    "course_enrollment_id"
                },
                values: new object[]
                {
                    400,
                    1,
                    totalPrice,
                    0,
                    300,
                    null
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "payments", keyColumn: "id", keyValue: 400);

            migrationBuilder.DeleteData(table: "hall_rentals", keyColumn: "id", keyValue: 300);

            migrationBuilder.DeleteData(table: "course_enrollments", keyColumn: "id", keyValue: 200);
            migrationBuilder.DeleteData(table: "course_enrollments", keyColumn: "id", keyValue: 201);
            migrationBuilder.DeleteData(table: "course_enrollments", keyColumn: "id", keyValue: 202);

            migrationBuilder.DeleteData(table: "course_sessions", keyColumn: "id", keyValue: 100);
            migrationBuilder.DeleteData(table: "course_sessions", keyColumn: "id", keyValue: 101);
            migrationBuilder.DeleteData(table: "course_sessions", keyColumn: "id", keyValue: 110);

            migrationBuilder.DeleteData(table: "courses", keyColumn: "id", keyValue: 10);
            migrationBuilder.DeleteData(table: "courses", keyColumn: "id", keyValue: 11);

        }
    }
}
