using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zerphin.RentACar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntityIdsToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys first (including shadow properties)
            migrationBuilder.Sql(@"
                -- Drop all foreign keys from Rentals
                DECLARE @sql NVARCHAR(MAX) = '';
                SELECT @sql += 'ALTER TABLE [Rentals] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                FROM sys.foreign_keys 
                WHERE parent_object_id = OBJECT_ID('Rentals');
                EXEC sp_executesql @sql;
                
                -- Drop all foreign keys from Payments
                SET @sql = '';
                SELECT @sql += 'ALTER TABLE [Payments] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                FROM sys.foreign_keys 
                WHERE parent_object_id = OBJECT_ID('Payments');
                EXEC sp_executesql @sql;
                
                -- Drop all foreign keys from Invoices
                SET @sql = '';
                SELECT @sql += 'ALTER TABLE [Invoices] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                FROM sys.foreign_keys 
                WHERE parent_object_id = OBJECT_ID('Invoices');
                EXEC sp_executesql @sql;
                
                -- Drop all foreign keys from Insurances
                SET @sql = '';
                SELECT @sql += 'ALTER TABLE [Insurances] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                FROM sys.foreign_keys 
                WHERE parent_object_id = OBJECT_ID('Insurances');
                EXEC sp_executesql @sql;
                
                -- Drop all foreign keys from Customers
                SET @sql = '';
                SELECT @sql += 'ALTER TABLE [Customers] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                FROM sys.foreign_keys 
                WHERE parent_object_id = OBJECT_ID('Customers');
                EXEC sp_executesql @sql;
                
                -- Drop all foreign keys from RefreshTokens
                SET @sql = '';
                SELECT @sql += 'ALTER TABLE [RefreshTokens] DROP CONSTRAINT ' + QUOTENAME(name) + ';'
                FROM sys.foreign_keys 
                WHERE parent_object_id = OBJECT_ID('RefreshTokens');
                EXEC sp_executesql @sql;
            ");

            // Delete existing data from dependent tables first, then Users
            migrationBuilder.Sql(@"
                DELETE FROM [RefreshTokens];
                DELETE FROM [Customers];
                DELETE FROM [Rentals];
                DELETE FROM [Payments];
                DELETE FROM [Invoices];
                DELETE FROM [Insurances];
                DELETE FROM [Users];
            ");

            // Drop and recreate Id columns with Guid type (IDENTITY columns need to be dropped first)
            migrationBuilder.Sql(@"
                -- Vehicles
                ALTER TABLE [Vehicles] DROP CONSTRAINT [PK_Vehicles];
                ALTER TABLE [Vehicles] DROP COLUMN [Id];
                ALTER TABLE [Vehicles] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Vehicles] ADD CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id]);
                
                -- Users  
                ALTER TABLE [Users] DROP CONSTRAINT [PK_Users];
                ALTER TABLE [Users] DROP COLUMN [Id];
                ALTER TABLE [Users] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY ([Id]);
                
                -- Rentals
                ALTER TABLE [Rentals] DROP CONSTRAINT [PK_Rentals];
                ALTER TABLE [Rentals] DROP COLUMN [Id];
                ALTER TABLE [Rentals] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Rentals] ADD CONSTRAINT [PK_Rentals] PRIMARY KEY ([Id]);
                
                -- Payments
                ALTER TABLE [Payments] DROP CONSTRAINT [PK_Payments];
                ALTER TABLE [Payments] DROP COLUMN [Id];
                ALTER TABLE [Payments] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Payments] ADD CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]);
                
                -- Invoices
                ALTER TABLE [Invoices] DROP CONSTRAINT [PK_Invoices];
                ALTER TABLE [Invoices] DROP COLUMN [Id];
                ALTER TABLE [Invoices] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Invoices] ADD CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]);
                
                -- Insurances
                ALTER TABLE [Insurances] DROP CONSTRAINT [PK_Insurances];
                ALTER TABLE [Insurances] DROP COLUMN [Id];
                ALTER TABLE [Insurances] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Insurances] ADD CONSTRAINT [PK_Insurances] PRIMARY KEY ([Id]);
                
                -- Customers
                ALTER TABLE [Customers] DROP CONSTRAINT [PK_Customers];
                ALTER TABLE [Customers] DROP COLUMN [Id];
                ALTER TABLE [Customers] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [Customers] ADD CONSTRAINT [PK_Customers] PRIMARY KEY ([Id]);
                
                -- RefreshTokens
                ALTER TABLE [RefreshTokens] DROP CONSTRAINT [PK_RefreshTokens];
                ALTER TABLE [RefreshTokens] DROP COLUMN [Id];
                ALTER TABLE [RefreshTokens] ADD [Id] uniqueidentifier NOT NULL DEFAULT NEWID();
                ALTER TABLE [RefreshTokens] ADD CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]);
            ");

            // Alter foreign key columns (drop and recreate to change type)
            migrationBuilder.Sql(@"
                -- Rentals: VehicleId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_VehicleId' AND object_id = OBJECT_ID('Rentals'))
                    DROP INDEX [IX_Rentals_VehicleId] ON [Rentals];
                DECLARE @var0 sysname;
                SELECT @var0 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rentals]') AND [c].[name] = N'VehicleId');
                IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Rentals] DROP CONSTRAINT [' + @var0 + '];');
                ALTER TABLE [Rentals] DROP COLUMN [VehicleId];
                ALTER TABLE [Rentals] ADD [VehicleId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE INDEX [IX_Rentals_VehicleId] ON [Rentals] ([VehicleId]);
                
                -- Rentals: CustomerId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rentals_CustomerId' AND object_id = OBJECT_ID('Rentals'))
                    DROP INDEX [IX_Rentals_CustomerId] ON [Rentals];
                DECLARE @var1 sysname;
                SELECT @var1 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rentals]') AND [c].[name] = N'CustomerId');
                IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Rentals] DROP CONSTRAINT [' + @var1 + '];');
                ALTER TABLE [Rentals] DROP COLUMN [CustomerId];
                ALTER TABLE [Rentals] ADD [CustomerId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE INDEX [IX_Rentals_CustomerId] ON [Rentals] ([CustomerId]);
                
                -- Payments: RentalId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payments_RentalId' AND object_id = OBJECT_ID('Payments'))
                    DROP INDEX [IX_Payments_RentalId] ON [Payments];
                DECLARE @var2 sysname;
                SELECT @var2 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'RentalId');
                IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var2 + '];');
                ALTER TABLE [Payments] DROP COLUMN [RentalId];
                ALTER TABLE [Payments] ADD [RentalId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE INDEX [IX_Payments_RentalId] ON [Payments] ([RentalId]);
                
                -- Invoices: RentalId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_RentalId' AND object_id = OBJECT_ID('Invoices'))
                    DROP INDEX [IX_Invoices_RentalId] ON [Invoices];
                DECLARE @var3 sysname;
                SELECT @var3 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Invoices]') AND [c].[name] = N'RentalId');
                IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Invoices] DROP CONSTRAINT [' + @var3 + '];');
                ALTER TABLE [Invoices] DROP COLUMN [RentalId];
                ALTER TABLE [Invoices] ADD [RentalId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE INDEX [IX_Invoices_RentalId] ON [Invoices] ([RentalId]);
                
                -- Insurances: VehicleId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Insurances_VehicleId' AND object_id = OBJECT_ID('Insurances'))
                    DROP INDEX [IX_Insurances_VehicleId] ON [Insurances];
                DECLARE @var4 sysname;
                SELECT @var4 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Insurances]') AND [c].[name] = N'VehicleId');
                IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Insurances] DROP CONSTRAINT [' + @var4 + '];');
                ALTER TABLE [Insurances] DROP COLUMN [VehicleId];
                ALTER TABLE [Insurances] ADD [VehicleId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE INDEX [IX_Insurances_VehicleId] ON [Insurances] ([VehicleId]);
                
                -- Customers: UserId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Customers_UserId' AND object_id = OBJECT_ID('Customers'))
                    DROP INDEX [IX_Customers_UserId] ON [Customers];
                DECLARE @var5 sysname;
                SELECT @var5 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'UserId');
                IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var5 + '];');
                ALTER TABLE [Customers] DROP COLUMN [UserId];
                ALTER TABLE [Customers] ADD [UserId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE UNIQUE INDEX [IX_Customers_UserId] ON [Customers] ([UserId]);
                
                -- RefreshTokens: UserId
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID('RefreshTokens'))
                    DROP INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens];
                DECLARE @var6 sysname;
                SELECT @var6 = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RefreshTokens]') AND [c].[name] = N'UserId');
                IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [RefreshTokens] DROP CONSTRAINT [' + @var6 + '];');
                ALTER TABLE [RefreshTokens] DROP COLUMN [UserId];
                ALTER TABLE [RefreshTokens] ADD [UserId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
            ");

            // Insert seed data for Users BEFORE creating foreign keys
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BirthDate", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "FirstName", "IdentityNumber", "IsActive", "IsDeleted", "LastLoginAt", "LastName", "PasswordHash", "PhoneNumber", "Role", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Istanbul, Turkey", new DateTime(1985, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 42, 24, 580, DateTimeKind.Utc).AddTicks(7454), "System", null, null, "admin@zerphinrentacar.com", "Admin", "12345678901", true, false, null, "User", "hVHi9RgZs8Lq5Wo08HmtGCSdKZiYIhwaHG1vThTgpwg0IGObokCTeES+6CFoLDKG", "+905551234567", 4, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Ankara, Turkey", new DateTime(1988, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 42, 24, 582, DateTimeKind.Utc).AddTicks(7503), "System", null, null, "manager@zerphinrentacar.com", "Manager", "12345678902", true, false, null, "User", "JMSnN4mgnNVTCNSAar28QIF7EvYAq3uwMN2kSJlG2tJFje3p7CKMDE7CPY9nff4K", "+905551234568", 3, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Izmir, Turkey", new DateTime(1990, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 42, 24, 584, DateTimeKind.Utc).AddTicks(4926), "System", null, null, "employee@zerphinrentacar.com", "Employee", "12345678903", true, false, null, "User", "LNx+YKKubczlkJCXWm9pyJ8Bdk3GBhqVd/z6UPnNbKz804PXYFdTxnSjNOyTwTLx", "+905551234569", 2, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Bursa, Turkey", new DateTime(1992, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 42, 24, 586, DateTimeKind.Utc).AddTicks(1910), "System", null, null, "john.doe@email.com", "John", "12345678904", true, false, null, "Doe", "HK4Sg1RnzgPf6J9f03c2hLHyU7ru+ooiMw7090LqfXP94lLB2VLtUU7rdd8WV1J8", "+905551234570", 1, null, null },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Antalya, Turkey", new DateTime(1995, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 42, 24, 587, DateTimeKind.Utc).AddTicks(8872), "System", null, null, "jane.smith@email.com", "Jane", "12345678905", true, false, null, "Smith", "o8hdYtIlyqjMG1ZYVviuR5kg/M+cq9yw7IfxfUvpHmlfwvKM2jb0VkhTzrMWK+SC", "+905551234571", 1, null, null }
                });

            // Recreate foreign keys
            migrationBuilder.Sql(@"
                ALTER TABLE [Rentals] ADD CONSTRAINT [FK_Rentals_Vehicles_VehicleId] 
                    FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles]([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Rentals] ADD CONSTRAINT [FK_Rentals_Users_CustomerId] 
                    FOREIGN KEY ([CustomerId]) REFERENCES [Users]([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Rentals_RentalId] 
                    FOREIGN KEY ([RentalId]) REFERENCES [Rentals]([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Invoices] ADD CONSTRAINT [FK_Invoices_Rentals_RentalId] 
                    FOREIGN KEY ([RentalId]) REFERENCES [Rentals]([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Insurances] ADD CONSTRAINT [FK_Insurances_Vehicles_VehicleId] 
                    FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles]([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Customers] ADD CONSTRAINT [FK_Customers_Users_UserId] 
                    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE;
                ALTER TABLE [RefreshTokens] ADD CONSTRAINT [FK_RefreshTokens_Users_UserId] 
                    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Vehicles",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "Rentals",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId1",
                table: "Rentals",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Rentals",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Rentals",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RefreshTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RefreshTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "RentalId",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "RentalId",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "Insurances",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Insurances",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BirthDate", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "FirstName", "IdentityNumber", "IsActive", "IsDeleted", "LastLoginAt", "LastName", "PasswordHash", "PhoneNumber", "Role", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Istanbul, Turkey", new DateTime(1985, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 27, 34, 762, DateTimeKind.Utc).AddTicks(235), "System", null, null, "admin@zerphinrentacar.com", "Admin", "12345678901", true, false, null, "User", "e+PQMd23KHy9F5pUQjxZadBxqb+4qGSvl4vm2fWLn4dRepiie33puqjXhm3tQbX9", "+905551234567", 4, null, null },
                    { 2, "Ankara, Turkey", new DateTime(1988, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 27, 34, 764, DateTimeKind.Utc).AddTicks(1298), "System", null, null, "manager@zerphinrentacar.com", "Manager", "12345678902", true, false, null, "User", "HAGQW4n4pHgGtLIX/QvPSKalk1ZmVN+6Dkf6jMbBDz+F/D/8hjXaxMtv2oTi0Pgp", "+905551234568", 3, null, null },
                    { 3, "Izmir, Turkey", new DateTime(1990, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 27, 34, 765, DateTimeKind.Utc).AddTicks(8584), "System", null, null, "employee@zerphinrentacar.com", "Employee", "12345678903", true, false, null, "User", "Ni01aEcoRm4uJ/OAvJlPDqZuO8XgIjpEBIl4cGqqLKfXRcCjX9moBZ5ibGv0sWpl", "+905551234569", 2, null, null },
                    { 4, "Bursa, Turkey", new DateTime(1992, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 27, 34, 767, DateTimeKind.Utc).AddTicks(5521), "System", null, null, "john.doe@email.com", "John", "12345678904", true, false, null, "Doe", "goRmWew0rWLUQtUsbKAkvP546pT7tQCWnudKcbf1CeKu+UC6kmzyaLt9dURaOxUB", "+905551234570", 1, null, null },
                    { 5, "Antalya, Turkey", new DateTime(1995, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 2, 19, 27, 34, 769, DateTimeKind.Utc).AddTicks(2515), "System", null, null, "jane.smith@email.com", "Jane", "12345678905", true, false, null, "Smith", "5vqM82bu4K/3KlnXqS0wOBeGVFmLycw8h71fXST56HtqISXd3hHo/NO0AXNLnEQQ", "+905551234571", 1, null, null }
                });
        }
    }
}
