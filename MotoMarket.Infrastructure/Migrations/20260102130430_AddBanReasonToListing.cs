using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBanReasonToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BanReason",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanReason",
                table: "Listings");
        }
    }
}
