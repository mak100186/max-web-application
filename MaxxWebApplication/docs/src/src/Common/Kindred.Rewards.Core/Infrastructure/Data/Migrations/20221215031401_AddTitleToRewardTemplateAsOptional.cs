using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class AddTitleToRewardTemplateAsOptional : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Title",
            table: "RewardTemplate",
            type: "character varying(300)",
            maxLength: 300,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Title",
            table: "RewardTemplate");
    }
}
