using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class RenameRewardCategoryFreeBetToRiskFree : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Reward",
            keyColumn: "Category",
            keyValue: "FreeBet",
            column: "Category",
            value: "RiskFree");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Reward",
            keyColumn: "Category",
            keyValue: "RiskFree",
            column: "Category",
            value: "FreeBet");
    }
}
