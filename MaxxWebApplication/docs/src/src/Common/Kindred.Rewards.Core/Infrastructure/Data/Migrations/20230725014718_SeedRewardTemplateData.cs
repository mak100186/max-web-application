using Microsoft.EntityFrameworkCore.Migrations;


#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class SeedRewardTemplateData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "RewardTemplate",
            columns: new[] { "Id", "Comments", "CreatedBy", "CreatedOn", "Enabled", "Key", "Title", "UpdatedBy", "UpdatedOn" },
            values: new object[,]
            {
                { 1, "UB_NEW", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8186), true, "UB_NEW", "UB_NEW", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8189) },
                { 2, "UB_STANDARD", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8191), true, "UB_STANDARD", "UB_STANDARD", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8192) },
                { 3, "UB_VALUED_SMALL", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8193), true, "UB_VALUED_SMALL", "UB_VALUED_SMALL", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8193) },
                { 4, "UB_VALUED_MEDIUM", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8194), true, "UB_VALUED_MEDIUM", "UB_VALUED_MEDIUM", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8195) },
                { 5, "UB_VALUED_LARGE", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8196), true, "UB_VALUED_LARGE", "UB_VALUED_LARGE", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8196) },
                { 6, "UB_VIP_SMALL", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8199), true, "UB_VIP_SMALL", "UB_VIP_SMALL", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8199) },
                { 7, "UB_VIP_MEDIUM", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8201), true, "UB_VIP_MEDIUM", "UB_VIP_MEDIUM", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8201) },
                { 8, "UB_VIP_LARGE", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8202), true, "UB_VIP_LARGE", "UB_VIP_LARGE", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8203) },
                { 9, "NO_REWARDS", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8204), true, "NO_REWARDS", "NO_REWARDS", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8204) },
                { 10, "UB_NO_REWARDS_FALLBACK", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8205), true, "UB_NO_REWARDS_FALLBACK", "UB_NO_REWARDS_FALLBACK", null, new DateTime(2023, 7, 25, 1, 47, 18, 358, DateTimeKind.Utc).AddTicks(8205) }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "RewardTemplate",
            keyColumn: "Id",
            keyValue: 10);
    }
}
