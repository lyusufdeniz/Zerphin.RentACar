using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomerEntityAddCustomerDetailsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Rentals_Customers_CustomerId1')
                BEGIN
                    ALTER TABLE [Rentals] DROP CONSTRAINT [FK_Rentals_Customers_CustomerId1];
                END
            ");

            // Drop index if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId1' AND object_id = OBJECT_ID('Rentals'))
                BEGIN
                    DROP INDEX [IX_Rentals_CustomerId1] ON [Rentals];
                END
            ");

            // Drop column if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'CustomerId1' AND object_id = OBJECT_ID('Rentals'))
                BEGIN
                    ALTER TABLE [Rentals] DROP COLUMN [CustomerId1];
                END
            ");

            // Drop Customers table if exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('Customers', 'U') IS NOT NULL
                BEGIN
                    -- Drop foreign keys from Customers table
                    DECLARE @sql NVARCHAR(MAX) = '';
                    SELECT @sql += 'ALTER TABLE [Customers] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                    FROM sys.foreign_keys 
                    WHERE parent_object_id = OBJECT_ID('Customers');
                    EXEC sp_executesql @sql;
                    
                    -- Drop the table
                    DROP TABLE [Customers];
                END
            ");

            migrationBuilder.AddColumn<int>(
                name: "CreditScore",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasInsurance",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceCompany",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsurancePolicyNumber",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseClass",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpiryDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialNotes",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationDocument",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "EmergencyContactName", "EmergencyContactPhone", "InsuranceCompany", "InsurancePolicyNumber", "LicenseClass", "LicenseExpiryDate", "LicenseNumber", "PasswordHash", "SpecialNotes", "VerificationDate", "VerificationDocument" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 370, DateTimeKind.Utc).AddTicks(8737), null, null, null, null, null, null, null, "cMpwOiCnaSX5h5OW3XdmcqEXYcGqEzEd/TNSWAivF+P+NTpRR0CA2Pz5vhSvRpDM", null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "EmergencyContactName", "EmergencyContactPhone", "InsuranceCompany", "InsurancePolicyNumber", "LicenseClass", "LicenseExpiryDate", "LicenseNumber", "PasswordHash", "SpecialNotes", "VerificationDate", "VerificationDocument" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 372, DateTimeKind.Utc).AddTicks(6756), null, null, null, null, null, null, null, "djP5TR1BYS1wqKbfABdwlx9JQVAstMsXHpTgpXNMeMolKrwo9WMhTHUPlQDjLMCl", null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "EmergencyContactName", "EmergencyContactPhone", "InsuranceCompany", "InsurancePolicyNumber", "LicenseClass", "LicenseExpiryDate", "LicenseNumber", "PasswordHash", "SpecialNotes", "VerificationDate", "VerificationDocument" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 374, DateTimeKind.Utc).AddTicks(3956), null, null, null, null, null, null, null, "h42A3MNVuaQIB1aPLkQnSIf7PunFnsN3zznuch4qtTuvo48eDzMB87CWbP34q0FY", null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "EmergencyContactName", "EmergencyContactPhone", "InsuranceCompany", "InsurancePolicyNumber", "LicenseClass", "LicenseExpiryDate", "LicenseNumber", "PasswordHash", "SpecialNotes", "VerificationDate", "VerificationDocument" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 376, DateTimeKind.Utc).AddTicks(994), null, null, null, null, null, null, null, "DLW6l1yahJkGJEHb1ZfL4MCxlWxYsF+4V74D/XgvdBFzLyGMrHFw0HFwW8D1OeNn", null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "EmergencyContactName", "EmergencyContactPhone", "InsuranceCompany", "InsurancePolicyNumber", "LicenseClass", "LicenseExpiryDate", "LicenseNumber", "PasswordHash", "SpecialNotes", "VerificationDate", "VerificationDocument" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 377, DateTimeKind.Utc).AddTicks(9099), null, null, null, null, null, null, null, "OrjgGm8x/i1TWUsxezuvvz7n6yOwEEuX21gbdr+D63y2Hufp/PJpyUJvqRBe8B0V", null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditScore",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HasInsurance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsuranceCompany",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InsurancePolicyNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenseClass",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenseExpiryDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpecialNotes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationDocument",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId1",
                table: "Rentals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HasInsurance = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    InsuranceCompany = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LicenseClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SpecialNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationDocument = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 15, 21, 27, 237, DateTimeKind.Utc).AddTicks(7797), "d9Cn5nwiM9dCDCCDDduoQh0bUbrrC3qRCmEgAErtlbf5KIzLHPe4U2FMZUUjc/I4" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 15, 21, 27, 239, DateTimeKind.Utc).AddTicks(5602), "hKZY5PqUzzQH7rXQhjAjZglHA+8l7JkJaOPtfiK9C477PgaiUP0FA9XSAFLqVahY" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 15, 21, 27, 241, DateTimeKind.Utc).AddTicks(2424), "P7hVkoGFjkxOwV8KOIWVYKlwqNU8GNsazjgHfUXoTtHPXB/ig1FIiIwE1ijnM6Wg" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 15, 21, 27, 242, DateTimeKind.Utc).AddTicks(9323), "W5itn9+OrnvFx2KDbwi408wqOP/NS7Z8B+T1NtuKBUCMX0kSQNyEDs2s/JVTRkOf" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 15, 21, 27, 244, DateTimeKind.Utc).AddTicks(6216), "WAdwcCGlKcnic9zZ3BJAgD4zX5/0TlvxfqDfx5puYvUb0hR1eHpWppa8xD+e79K2" });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId1",
                table: "Rentals",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Customers_CustomerId1",
                table: "Rentals",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
