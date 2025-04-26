using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PriceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<decimal>(
                name: "LivingPrice",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LivingPrice",
                table: "Reservations");

            migrationBuilder.InsertData(
                table: "ReservationStatuses",
                columns: new[] { "ID", "Status" },
                values: new object[] { 1, "Ожидание" });

            migrationBuilder.InsertData(
                table: "RoomCategories",
                columns: new[] { "ID", "Category" },
                values: new object[] { 1, "Стандарт" });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "ID", "Description", "GuestCapacity", "Price" },
                values: new object[] { 1, "", 1, 1000m });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "ID", "Number" },
                values: new object[] { 1, 101 });
        }
    }
}
