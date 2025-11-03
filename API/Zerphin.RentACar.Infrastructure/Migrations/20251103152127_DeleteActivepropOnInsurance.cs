using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteActivepropOnInsurance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Insurances");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Insurances",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 14, 46, 27, 997, DateTimeKind.Utc).AddTicks(6858), "HNs8kNtSm/Pd7QBLfuXkWwT3pKUavIadI8FUU5Ec3Fb6TsuUCgF8EgPBy73jGvZC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 14, 46, 27, 999, DateTimeKind.Utc).AddTicks(5496), "tG5ShKsnB/jts7hILNcRQlzykqOGsSgReTZkIdlIaLMMmUHQsSbTTH5asYorBipJ" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 14, 46, 28, 1, DateTimeKind.Utc).AddTicks(2752), "XHFqnsJzRGW6E4sld/cVViNan52pCAcaLFvjg9WhyxsmrJ1F0ubIgxi1WYzskB3R" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 14, 46, 28, 3, DateTimeKind.Utc).AddTicks(160), "kU9lg2dE+CWF6beaM2sLefvWSsy9fD+GuYtDbpT6U+fkSWMZSDV6XPgX+dTiv4DW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 14, 46, 28, 4, DateTimeKind.Utc).AddTicks(6965), "1f6zmO6RsFg5bHFxDlbD1fi3v4PHw2b2ls5pdH6PP7P7L7I2KZMHfFDYsdYzuSqi" });
        }
    }
}
