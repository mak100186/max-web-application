using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class RenamePromoIdToNamingConvention : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PromoId",
            table: "RewardClaim",
            newName: "NamingConvention");

        migrationBuilder.RenameColumn(
            name: "PromoId",
            table: "Reward",
            newName: "NamingConvention");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "NamingConvention",
            table: "RewardClaim",
            newName: "PromoId");

        migrationBuilder.RenameColumn(
            name: "NamingConvention",
            table: "Reward",
            newName: "PromoId");
    }
}
