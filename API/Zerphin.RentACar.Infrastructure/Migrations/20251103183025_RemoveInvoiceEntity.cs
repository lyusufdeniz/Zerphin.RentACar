using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInvoiceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint from Rentals table to Invoices table if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_Rentals_RentalId')
                BEGIN
                    ALTER TABLE [Invoices] DROP CONSTRAINT [FK_Invoices_Rentals_RentalId];
                END
            ");

            // Drop index on InvoiceNumber if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_InvoiceNumber' AND object_id = OBJECT_ID('Invoices'))
                BEGIN
                    DROP INDEX [IX_Invoices_InvoiceNumber] ON [Invoices];
                END
            ");

            // Drop index on RentalId if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_RentalId' AND object_id = OBJECT_ID('Invoices'))
                BEGIN
                    DROP INDEX [IX_Invoices_RentalId] ON [Invoices];
                END
            ");

            // Drop the Invoices table
            migrationBuilder.Sql(@"
                IF OBJECT_ID('Invoices', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Invoices];
                END
            ");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 30, 22, 379, DateTimeKind.Utc).AddTicks(1381), "nJaDz/B4PIIqm5YvVXVe9PyxNfLoHXbNSWJz/LlOVUdGfhXWd12/qyDQhr5hv+nn" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 30, 22, 381, DateTimeKind.Utc).AddTicks(2785), "qL3aRRGgvDjUeo0Cx2wNRN8bHpvwX9EG9+e0T43/tPmrCg1Tcesd64JY9ODlJq7s" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 30, 22, 383, DateTimeKind.Utc).AddTicks(1179), "CBAR6njPwfWTKw5lSmIZegiUHl+VSFBtjJgsevGAtA6VnMQD4lecOsbuEFOOR4xy" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 30, 22, 385, DateTimeKind.Utc).AddTicks(4397), "qev7L23CvD32lZpBWH9foujs1NJBGdrCipn+WEUutoaCNP3ln2mTpbfKirpM5aHw" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 30, 22, 387, DateTimeKind.Utc).AddTicks(2080), "Nv0EcsALDrYg2EpBrb0//HCv5jzZMQIO2DHfU5gbb4VMcEuKsBcO647UqtCBYSEz" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: When rolling back, the Invoices table will be recreated but all data will be lost
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BillingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CustomerTaxNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RentalId",
                table: "Invoices",
                column: "RentalId",
                unique: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 6, 17, 572, DateTimeKind.Utc).AddTicks(2775), "N5AN7t/ei0ZRlRcGeb4yx3W81tWYEBmh4U1uROjXMwXRJ6TzAMfg84WATyyt3ChH" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 6, 17, 574, DateTimeKind.Utc).AddTicks(2320), "ACEHxfOGBsvw6Xzix0idj6ic0sPepJjj94TI95rO/njQPduF3kfJwHptjlRUNvc3" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 6, 17, 576, DateTimeKind.Utc).AddTicks(3237), "TL0tbKevmTYUL0cfGcqM5ory2sCA+eee09mVML+QF7xEBtWqCZfcBuR+Bnk15TO0" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 6, 17, 578, DateTimeKind.Utc).AddTicks(576), "yjZAHO7mWrJutHymDw11lKDxIkEFNLpAJJqVNXvjDQm9k4gYc0kCWbzW3qixJsES" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 18, 6, 17, 580, DateTimeKind.Utc).AddTicks(769), "Nez2Twm3UHWb5zdZEXQx5+94ZxeeZIeiBt79rLSAgakylFXWgLw1XpkjLVuqricU" });
        }
    }
}
