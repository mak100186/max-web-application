using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Rewards.Repositories.Migrations;

public partial class AddIsLockedFlag : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsLocked",
            table: "Reward",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsLocked",
            table: "Reward");
    }
}
