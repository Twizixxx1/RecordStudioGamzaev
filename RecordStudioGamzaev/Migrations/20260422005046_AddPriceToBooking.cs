using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStudioGamzaev.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Bookings");
        }
    }
}
