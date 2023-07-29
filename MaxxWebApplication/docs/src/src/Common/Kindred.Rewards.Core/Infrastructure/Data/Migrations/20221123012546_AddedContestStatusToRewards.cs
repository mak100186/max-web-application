using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class AddedContestStatusToRewards : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ContestStatus",
            table: "Reward",
            type: "character varying(10)",
            unicode: false,
            maxLength: 10,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ContestStatus",
            table: "Reward");
    }
}
