using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentalStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update RentalStatus enum values in database
            // Old: Pending=1, Confirmed=2, Active=3, Completed=4, Cancelled=5, Overdue=6
            // New: Active=1, Completed=2, Cancelled=3
            // Map old values to new values:
            // Pending (1) -> Active (1) - already correct
            // Confirmed (2) -> Active (1)
            // Active (3) -> Active (1)
            // Completed (4) -> Completed (2)
            // Cancelled (5) -> Cancelled (3)
            // Overdue (6) -> Completed (2)
            migrationBuilder.Sql(@"
                UPDATE Rentals 
                SET Status = CASE 
                    WHEN Status = 1 THEN 1  -- Pending -> Active
                    WHEN Status = 2 THEN 1  -- Confirmed -> Active
                    WHEN Status = 3 THEN 1  -- Active -> Active
                    WHEN Status = 4 THEN 2  -- Completed -> Completed
                    WHEN Status = 5 THEN 3  -- Cancelled -> Cancelled
                    WHEN Status = 6 THEN 2  -- Overdue -> Completed
                    ELSE 1
                END
            ");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: When rolling back, we cannot restore the original status values exactly
            // because we don't know which Active rentals were Pending vs Confirmed vs old Active
            // They will be restored to Active (3), and Overdue will remain as Completed
            migrationBuilder.Sql(@"
                UPDATE Rentals 
                SET Status = CASE 
                    WHEN Status = 1 THEN 3  -- Active -> Active (old enum value)
                    WHEN Status = 2 THEN 4  -- Completed -> Completed (old enum value)
                    WHEN Status = 3 THEN 5  -- Cancelled -> Cancelled (old enum value)
                    ELSE 1
                END
            ");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 17, 53, 34, 251, DateTimeKind.Utc).AddTicks(3759), "ujZlr5f+4pqNkA18fl+HEIx/2Vp5WyWRZdk6loz7rndy67PDWyQc2keJALj0dYnr" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 17, 53, 34, 253, DateTimeKind.Utc).AddTicks(5218), "L88T2xCh5hlPzMC82uw96irjHrSUwVOl6XNGzoNPti98wTRDRV+k0OCq61sKzxjB" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 17, 53, 34, 255, DateTimeKind.Utc).AddTicks(3277), "wIg7/BbER/6XWgeS7pr9gTLqwE8kk1uHZLB3XQm1q7kvcSgLMNs/2o6Y99i0+nP2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 17, 53, 34, 257, DateTimeKind.Utc).AddTicks(2977), "N5GYkrlwA31HTSmKwz3ZT7GzzJ1C4hdUWS3784L37qvgpiQdFWZq/lIkor3pafNs" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 17, 53, 34, 259, DateTimeKind.Utc).AddTicks(863), "4ZfwuuAp/epqp5m+8Z7znOG9VN2tOnRMycizH3MmpBbJUDQ3TA0Z33iACfCbqa1C" });
        }
    }
}
