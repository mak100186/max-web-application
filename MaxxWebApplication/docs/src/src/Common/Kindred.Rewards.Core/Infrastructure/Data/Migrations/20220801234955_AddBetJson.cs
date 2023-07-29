using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class AddBetJson : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "RewardKey",
            table: "RewardClaim",
            newName: "RewardRn");

        migrationBuilder.RenameColumn(
            name: "CouponRef",
            table: "RewardClaim",
            newName: "CouponRn");

        migrationBuilder.RenameColumn(
            name: "BetRef",
            table: "RewardClaim",
            newName: "BetRn");

        migrationBuilder.AddColumn<string>(
            name: "BetJson",
            table: "RewardClaim",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BetJson",
            table: "RewardClaim");

        migrationBuilder.RenameColumn(
            name: "RewardRn",
            table: "RewardClaim",
            newName: "RewardKey");

        migrationBuilder.RenameColumn(
            name: "CouponRn",
            table: "RewardClaim",
            newName: "CouponRef");

        migrationBuilder.RenameColumn(
            name: "BetRn",
            table: "RewardClaim",
            newName: "BetRef");
    }
}
