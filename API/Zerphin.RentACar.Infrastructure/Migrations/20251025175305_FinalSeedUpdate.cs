using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalSeedUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7458));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7785));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7787));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 53, 4, 768, DateTimeKind.Utc).AddTicks(7788));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 776, DateTimeKind.Utc).AddTicks(3854), "RvNq626LIO+g+G6eaVojJcpWlqSFSriN4w0AZwZWX5PG64JvfUYKrdm0U12ijWmP" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 778, DateTimeKind.Utc).AddTicks(1454), "ywexChNf46pWg7ITnZHRwyKX3aNTy0UZ+OKnufablZ3lagSh6fe59DD1zJXRrnth" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 779, DateTimeKind.Utc).AddTicks(8419), "Px65LMZ5rW1NUCfsoP3ta/XkWhYxMqgXOqNgW5492BAZQgMEkm2VPDilWEivvY5h" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 781, DateTimeKind.Utc).AddTicks(6232), "46sd8iohrXMlpr3e2K5Rbfo0VAD/cLENIzcqeRhdyQK7ZihyS3XI9ZztBIi+JY3f" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 53, 4, 783, DateTimeKind.Utc).AddTicks(3391), "bZy5G7LVxPEBpmhY9TeO0CUEtg2Ek49Soi1KKPKjPxDrEYCxLwDjsxevT+jeHvkF" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 30, 876, DateTimeKind.Utc).AddTicks(1804));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 30, 876, DateTimeKind.Utc).AddTicks(2134));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 30, 876, DateTimeKind.Utc).AddTicks(2178));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 30, 876, DateTimeKind.Utc).AddTicks(2180));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 30, 884, DateTimeKind.Utc).AddTicks(915), "v/ZV8fiqy9K7XFH6AWHWAAqGSLjgC52msj89ochnYCwWLXYvJFAnLvN5MqUHddHf" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 30, 885, DateTimeKind.Utc).AddTicks(8784), "Qrz0uNgbR5PQRH1VY72hSmBVLAKBOQuSUXOVyCkG0D8z+6i1e+VEk+qhoCbcAtzy" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 30, 887, DateTimeKind.Utc).AddTicks(5891), "Ho9zFEJROaGGpAxmkT2t+jPN+U6w0YovTx9HfhKHP5P3Gh656B4taAcO9EdFj5B5" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 30, 889, DateTimeKind.Utc).AddTicks(2869), "ToLYoQC8C4hN4ynDHnDt08tk707RuEr078BpsDCuBoSIQahtsluwceZe4sIXzzJJ" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 30, 890, DateTimeKind.Utc).AddTicks(9851), "E6nKld8Eut33lYQbBQjvNq+RSENTeXWF65eVBs8n2Y8XgX9+SL/1Lzf8CooUQiIb" });
        }
    }
}
