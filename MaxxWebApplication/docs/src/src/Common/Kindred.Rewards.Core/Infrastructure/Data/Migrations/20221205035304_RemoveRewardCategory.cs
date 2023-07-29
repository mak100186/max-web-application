using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class RemoveRewardCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Category",
            table: "Reward");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Category",
            table: "Reward",
            type: "character varying(30)",
            maxLength: 30,
            nullable: false,
            defaultValue: "");
    }
}
