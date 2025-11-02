using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleEntityUseUserRoleEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Locations_LocationId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_LocationId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Users",
                newName: "Role");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Invoices",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 27, 34, 762, DateTimeKind.Utc).AddTicks(235), "e+PQMd23KHy9F5pUQjxZadBxqb+4qGSvl4vm2fWLn4dRepiie33puqjXhm3tQbX9", 4 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 27, 34, 764, DateTimeKind.Utc).AddTicks(1298), "HAGQW4n4pHgGtLIX/QvPSKalk1ZmVN+6Dkf6jMbBDz+F/D/8hjXaxMtv2oTi0Pgp", 3 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 27, 34, 765, DateTimeKind.Utc).AddTicks(8584), "Ni01aEcoRm4uJ/OAvJlPDqZuO8XgIjpEBIl4cGqqLKfXRcCjX9moBZ5ibGv0sWpl", 2 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 27, 34, 767, DateTimeKind.Utc).AddTicks(5521), "goRmWew0rWLUQtUsbKAkvP546pT7tQCWnudKcbf1CeKu+UC6kmzyaLt9dURaOxUB", 1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 27, 34, 769, DateTimeKind.Utc).AddTicks(2515), "5vqM82bu4K/3KlnXqS0wOBeGVFmLycw8h71fXST56HtqISXd3hHo/NO0AXNLnEQQ", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "RoleId");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPickupLocation = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsReturnLocation = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7458), "System", null, null, "System Administrator", true, false, "Admin", null, null },
                    { 2, new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7785), "System", null, null, "Branch Manager", true, false, "Manager", null, null },
                    { 3, new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7787), "System", null, null, "Rental Employee", true, false, "Employee", null, null },
                    { 4, new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7788), "System", null, null, "Rental Customer", true, false, "Customer", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "RoleId" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 776, DateTimeKind.Utc).AddTicks(3854), "RvNq626LIO+g+G6eaVojJcpWlqSFSriN4w0AZwZWX5PG64JvfUYKrdm0U12ijWmP", 1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "RoleId" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 778, DateTimeKind.Utc).AddTicks(1454), "ywexChNf46pWg7ITnZHRwyKX3aNTy0UZ+OKnufablZ3lagSh6fe59DD1zJXRrnth", 2 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash", "RoleId" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 779, DateTimeKind.Utc).AddTicks(8419), "Px65LMZ5rW1NUCfsoP3ta/XkWhYxMqgXOqNgW5492BAZQgMEkm2VPDilWEivvY5h", 3 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash", "RoleId" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 781, DateTimeKind.Utc).AddTicks(6232), "46sd8iohrXMlpr3e2K5Rbfo0VAD/cLENIzcqeRhdyQK7ZihyS3XI9ZztBIi+JY3f", 4 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash", "RoleId" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 783, DateTimeKind.Utc).AddTicks(3391), "bZy5G7LVxPEBpmhY9TeO0CUEtg2Ek49Soi1KKPKjPxDrEYCxLwDjsxevT+jeHvkF", 4 });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LocationId",
                table: "Vehicles",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Locations_LocationId",
                table: "Vehicles",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
