using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class HarmoniseRewardKeyToRewardRn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameIndex(
            name: "IX_RewardTemplateReward_RewardKey",
            table: "RewardTemplateReward",
            newName: "IX_RewardTemplateReward_RewardRn");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameIndex(
            name: "IX_RewardTemplateReward_RewardRn",
            table: "RewardTemplateReward",
            newName: "IX_RewardTemplateReward_RewardKey");
    }
}
