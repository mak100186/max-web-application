using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class RenameRewardRnToRewardIdInRewardClaim : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "RewardRn",
            table: "RewardClaim",
            newName: "RewardId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "RewardId",
            table: "RewardClaim",
            newName: "RewardRn");
    }
}
