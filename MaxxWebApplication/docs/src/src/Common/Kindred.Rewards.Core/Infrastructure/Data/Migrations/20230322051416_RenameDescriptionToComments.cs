using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Rewards.Repositories.Migrations;

public partial class RenameDescriptionToComments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Description",
            table: "Tag");

        migrationBuilder.DropColumn(
            name: "Description",
            table: "RewardTemplate");

        migrationBuilder.DropColumn(
            name: "Description",
            table: "Reward");

        migrationBuilder.AddColumn<string>(
            name: "Comments",
            table: "Tag",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Comments",
            table: "RewardTemplate",
            type: "character varying(300)",
            maxLength: 300,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Comments",
            table: "Reward",
            type: "character varying(300)",
            maxLength: 300,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Comments",
            table: "Tag");

        migrationBuilder.DropColumn(
            name: "Comments",
            table: "RewardTemplate");

        migrationBuilder.DropColumn(
            name: "Comments",
            table: "Reward");

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Tag",
            type: "character varying(2000)",
            unicode: false,
            maxLength: 2000,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "RewardTemplate",
            type: "character varying(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Reward",
            type: "character varying(300)",
            maxLength: 300,
            nullable: true);
    }
}
