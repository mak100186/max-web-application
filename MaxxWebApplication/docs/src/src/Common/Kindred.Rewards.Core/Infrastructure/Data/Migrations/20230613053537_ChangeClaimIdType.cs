using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class ChangeClaimIdType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CombinationRewardPayoff_RewardClaim_Id",
            table: "CombinationRewardPayoff");

        migrationBuilder.DropColumn(
            name: "ClaimId",
            table: "CombinationRewardPayoff");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "CombinationRewardPayoff",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer")
            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        migrationBuilder.AddColumn<string>(
            name: "ClaimInstanceId",
            table: "CombinationRewardPayoff",
            type: "character varying(50)",
            nullable: true);

        migrationBuilder.AddUniqueConstraint(
            name: "AK_RewardClaim_InstanceId",
            table: "RewardClaim",
            column: "InstanceId");

        migrationBuilder.CreateIndex(
            name: "IX_CombinationRewardPayoff_ClaimInstanceId",
            table: "CombinationRewardPayoff",
            column: "ClaimInstanceId");

        migrationBuilder.AddForeignKey(
            name: "FK_CombinationRewardPayoff_RewardClaim_ClaimInstanceId",
            table: "CombinationRewardPayoff",
            column: "ClaimInstanceId",
            principalTable: "RewardClaim",
            principalColumn: "InstanceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CombinationRewardPayoff_RewardClaim_ClaimInstanceId",
            table: "CombinationRewardPayoff");

        migrationBuilder.DropUniqueConstraint(
            name: "AK_RewardClaim_InstanceId",
            table: "RewardClaim");

        migrationBuilder.DropIndex(
            name: "IX_CombinationRewardPayoff_ClaimInstanceId",
            table: "CombinationRewardPayoff");

        migrationBuilder.DropColumn(
            name: "ClaimInstanceId",
            table: "CombinationRewardPayoff");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "CombinationRewardPayoff",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer")
            .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        migrationBuilder.AddColumn<Guid>(
            name: "ClaimId",
            table: "CombinationRewardPayoff",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddForeignKey(
            name: "FK_CombinationRewardPayoff_RewardClaim_Id",
            table: "CombinationRewardPayoff",
            column: "Id",
            principalTable: "RewardClaim",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
