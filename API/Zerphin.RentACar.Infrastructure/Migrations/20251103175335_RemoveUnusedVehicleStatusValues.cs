using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedVehicleStatusValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update any vehicles with Maintenance (3) or OutOfService (4) status to Available (1)
            // Since we removed Maintenance and OutOfService from the enum, existing records need to be updated
            migrationBuilder.Sql(@"
                UPDATE Vehicles 
                SET Status = 1 
                WHERE Status IN (3, 4)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: When rolling back, we cannot restore the original status values 
            // because we don't know which vehicles were Maintenance vs OutOfService
            // They will remain as Available (1)

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 370, DateTimeKind.Utc).AddTicks(8737), "cMpwOiCnaSX5h5OW3XdmcqEXYcGqEzEd/TNSWAivF+P+NTpRR0CA2Pz5vhSvRpDM" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 372, DateTimeKind.Utc).AddTicks(6756), "djP5TR1BYS1wqKbfABdwlx9JQVAstMsXHpTgpXNMeMolKrwo9WMhTHUPlQDjLMCl" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 374, DateTimeKind.Utc).AddTicks(3956), "h42A3MNVuaQIB1aPLkQnSIf7PunFnsN3zznuch4qtTuvo48eDzMB87CWbP34q0FY" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 376, DateTimeKind.Utc).AddTicks(994), "DLW6l1yahJkGJEHb1ZfL4MCxlWxYsF+4V74D/XgvdBFzLyGMrHFw0HFwW8D1OeNn" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 3, 16, 16, 54, 377, DateTimeKind.Utc).AddTicks(9099), "OrjgGm8x/i1TWUsxezuvvz7n6yOwEEuX21gbdr+D63y2Hufp/PJpyUJvqRBe8B0V" });
        }
    }
}
