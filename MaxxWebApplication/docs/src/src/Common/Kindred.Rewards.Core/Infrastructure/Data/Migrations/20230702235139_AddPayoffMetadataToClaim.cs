using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore.Migrations;


namespace Kindred.Rewards.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddPayoffMetadataToClaim : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<RewardClaimPayoffMetadata>(
            name: "PayoffMetadata",
            table: "RewardClaim",
            type: "jsonb",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PayoffMetadata",
            table: "RewardClaim");
    }
}
