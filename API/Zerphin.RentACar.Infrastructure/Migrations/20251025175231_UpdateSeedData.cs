using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 16, 780, DateTimeKind.Utc).AddTicks(424));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 16, 780, DateTimeKind.Utc).AddTicks(752));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 16, 780, DateTimeKind.Utc).AddTicks(754));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 25, 17, 52, 16, 780, DateTimeKind.Utc).AddTicks(756));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 16, 788, DateTimeKind.Utc).AddTicks(2175), "hNjeJFlXngnm1Ren0rPbI3PvCCNxnB7w+beabsR2HBDfD8c6dlU+LgMqjzqCW4E3" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 16, 789, DateTimeKind.Utc).AddTicks(9936), "m7jig1UvML4ffEx4YtizVDP77RMgWRhvUYQrtkJ3N4MPzDDPPxAsAXSLgkuMrUpN" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 16, 791, DateTimeKind.Utc).AddTicks(7138), "FLKk/qFHKQrODpkFYXMABth9NaIbGBwOOaXHdn3vaF2/pr7Q4I4lZvhRVdr4jPHd" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 16, 793, DateTimeKind.Utc).AddTicks(4227), "8Ux7/Jlzm/AhM5VMAOuJz2lEtYM8tcgyVnIHH8N+370OZnI/+qr21jtqMnZ7ESzj" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 25, 17, 52, 16, 795, DateTimeKind.Utc).AddTicks(1280), "7E1nmZzEUQ8fBqJlchwB1PO6j7BcgTDd8TDJF6nNQd+fi6fOhLXt5UL9969CiYql" });
        }
    }
}
