using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingParametersNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "VehicleParameterTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VehicleModels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VehicleFeatures",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VehicleCategories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VehicleBrands",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PageContents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ListingViews",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Listings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ListingPhotos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ListingParameters",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ListingFeatures",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "GearboxTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FuelTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ErrorLogs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "DriveTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "BodyTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AuditLogs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AdminSettings",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleParameterTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleModels",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleFeatures",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleCategories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VehicleBrands",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PageContents",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ListingViews",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Listings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ListingPhotos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ListingParameters",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ListingFeatures",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GearboxTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FuelTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ErrorLogs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DriveTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BodyTypes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AuditLogs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AdminSettings",
                newName: "id");
        }
    }
}
