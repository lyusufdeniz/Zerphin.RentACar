using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleCategoryEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mevcut Vehicle category verilerini yeni enum değerlerine map et
            // Eski: Economy(1) -> Yeni: Hatchback(1)
            // Eski: Compact(2) -> Yeni: Sedan(2)
            // Eski: MidSize(3) -> Yeni: SUV(3)
            // Eski: FullSize(4) -> Yeni: Pickup(4)
            // Eski: Luxury(5) -> Yeni: Sedan(2) (default olarak sedan)
            // Eski: SUV(6) -> Yeni: SUV(3)
            // Eski: Van(7) -> Yeni: Pickup(4)
            // Eski: Truck(8) -> Yeni: Pickup(4)
            migrationBuilder.Sql(@"
                UPDATE Vehicles 
                SET Category = CASE 
                    WHEN Category = 1 THEN 1  -- Economy -> Hatchback
                    WHEN Category = 2 THEN 2  -- Compact -> Sedan
                    WHEN Category = 3 THEN 3  -- MidSize -> SUV
                    WHEN Category = 4 THEN 4  -- FullSize -> Pickup
                    WHEN Category = 5 THEN 2  -- Luxury -> Sedan
                    WHEN Category = 6 THEN 3  -- SUV -> SUV
                    WHEN Category = 7 THEN 4  -- Van -> Pickup
                    WHEN Category = 8 THEN 4  -- Truck -> Pickup
                    ELSE 1  -- Default to Hatchback
                END
                WHERE Category IN (1, 2, 3, 4, 5, 6, 7, 8);
            ");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Yeni enum değerlerini eski enum değerlerine map et
            // Yeni: Hatchback(1) -> Eski: Economy(1)
            // Yeni: Sedan(2) -> Eski: Compact(2)
            // Yeni: SUV(3) -> Eski: MidSize(3)
            // Yeni: Pickup(4) -> Eski: FullSize(4)
            migrationBuilder.Sql(@"
                UPDATE Vehicles 
                SET Category = CASE 
                    WHEN Category = 1 THEN 1  -- Hatchback -> Economy
                    WHEN Category = 2 THEN 2  -- Sedan -> Compact
                    WHEN Category = 3 THEN 3  -- SUV -> MidSize
                    WHEN Category = 4 THEN 4  -- Pickup -> FullSize
                    ELSE 1  -- Default to Economy
                END
                WHERE Category IN (1, 2, 3, 4);
            ");

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
        }
    }
}
