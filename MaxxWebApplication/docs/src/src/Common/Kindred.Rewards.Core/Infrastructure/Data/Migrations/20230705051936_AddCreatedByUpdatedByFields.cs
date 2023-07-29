using Microsoft.EntityFrameworkCore.Migrations;


namespace Kindred.Rewards.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddCreatedByUpdatedByFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "RewardTemplate",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "RewardTemplate",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "RewardTemplate");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "RewardTemplate");
    }
}
