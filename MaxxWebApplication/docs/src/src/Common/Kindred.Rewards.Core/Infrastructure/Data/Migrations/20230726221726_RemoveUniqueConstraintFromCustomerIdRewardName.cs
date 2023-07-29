using System;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Kindred.Rewards.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RemoveUniqueConstraintFromCustomerIdRewardName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Reward_CustomerName",
            table: "Reward");

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 1,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 2,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 3,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 4,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 5,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 6,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 7,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 8,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 9,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 10,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 1, 1, 1, 0, DateTimeKind.Utc) });

        migrationBuilder.CreateIndex(
            name: "IX_Reward_CustomerName",
            table: "Reward",
            columns: new[] { "CustomerId", "Name" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Reward_CustomerName",
            table: "Reward");

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 1,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8186), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8189) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 2,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8191), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8192) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 3,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8193), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8193) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 4,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8194), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8195) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 5,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8196), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8196) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 6,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8199), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8199) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 7,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8201), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8201) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 8,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8202), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8203) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 9,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8204), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8204) });

        migrationBuilder.UpdateData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 10,
            columns: new[] { "CreatedOn", "UpdatedOn" },
            values: new object[] { new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8205), new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8205) });

        migrationBuilder.CreateIndex(
            name: "IX_Reward_CustomerName",
            table: "Reward",
            columns: new[] { "CustomerId", "Name" },
            unique: true);
    }
}
