using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Rewards.Repositories.Migrations;

public partial class AddCurrencyCodeToRewards : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CurrencyCode",
            table: "Reward",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CurrencyCode",
            table: "Reward");
    }
}
