using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 20, 0, 17, 16, DateTimeKind.Utc).AddTicks(1555), "4KKTeIaebEiVlWPQP+v0qWL/73HY8DuYl7Isb2YiMmMI82uC4SIGNI3gwsWw9Mkk" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 20, 0, 17, 18, DateTimeKind.Utc).AddTicks(1241), "6Twu/AwPQKKSW1vpjRuLgzzEoGL+Fd0JI72mTn6NxTVoVeTFL/D4/38ZDcrCBBD+" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 20, 0, 17, 19, DateTimeKind.Utc).AddTicks(9493), "NlpuAx8uyneDcu5BqjRxpAcRtOtmC2wsHppY2QrHSFrRs8TcYuapLze7m7T6q3lu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 20, 0, 17, 21, DateTimeKind.Utc).AddTicks(9628), "8MqnppBOVTx7pHxgNg9JTQ1+Ab3dBhOqSSL8zyblxUMFOYVR3ikIbYLQnzq3wLKj" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 20, 0, 17, 23, DateTimeKind.Utc).AddTicks(8021), "07NA44zpFqMYrBWKVgExbvwWxHwAD8eQ3yMqX8n16DRW7UkOQaby/5zWoAClTlIh" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 42, 24, 580, DateTimeKind.Utc).AddTicks(7454), "hVHi9RgZs8Lq5Wo08HmtGCSdKZiYIhwaHG1vThTgpwg0IGObokCTeES+6CFoLDKG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 42, 24, 582, DateTimeKind.Utc).AddTicks(7503), "JMSnN4mgnNVTCNSAar28QIF7EvYAq3uwMN2kSJlG2tJFje3p7CKMDE7CPY9nff4K" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 42, 24, 584, DateTimeKind.Utc).AddTicks(4926), "LNx+YKKubczlkJCXWm9pyJ8Bdk3GBhqVd/z6UPnNbKz804PXYFdTxnSjNOyTwTLx" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 42, 24, 586, DateTimeKind.Utc).AddTicks(1910), "HK4Sg1RnzgPf6J9f03c2hLHyU7ru+ooiMw7090LqfXP94lLB2VLtUU7rdd8WV1J8" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 2, 19, 42, 24, 587, DateTimeKind.Utc).AddTicks(8872), "o8hdYtIlyqjMG1ZYVviuR5kg/M+cq9yw7IfxfUvpHmlfwvKM2jb0VkhTzrMWK+SC" });
        }
    }
}
