using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Rewards.Repositories.Migrations;

public partial class RemovedBackendTitle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BackendTitle",
            table: "Reward");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "BackendTitle",
            table: "Reward",
            type: "character varying(25)",
            maxLength: 25,
            nullable: true);
    }
}
