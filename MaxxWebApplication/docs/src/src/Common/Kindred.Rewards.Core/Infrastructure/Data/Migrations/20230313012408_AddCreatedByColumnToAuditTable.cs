using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Rewards.Repositories.Migrations;

public partial class AddCreatedByColumnToAuditTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "AuditReward",
            type: "character varying(100)",
            maxLength: 100,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "AuditReward");
    }
}
