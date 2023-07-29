using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class AddCombinationRewardPayoffTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Title",
            table: "RewardTemplate");

        migrationBuilder.AlterColumn<string>(
            name: "CurrencyCode",
            table: "Reward",
            type: "text",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "CombinationRewardPayoff",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                BetRn = table.Column<string>(type: "text", nullable: false),
                CombinationRn = table.Column<string>(type: "text", nullable: false),
                BetPayoff = table.Column<decimal>(type: "numeric", nullable: false),
                BetStake = table.Column<decimal>(type: "numeric", nullable: false),
                CombinationPayoff = table.Column<decimal>(type: "numeric", nullable: false),
                ClaimId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                UpdatedOn = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CombinationRewardPayoff", x => x.Id);
                table.ForeignKey(
                    name: "FK_CombinationRewardPayoff_RewardClaim_Id",
                    column: x => x.Id,
                    principalTable: "RewardClaim",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CombinationRewardPayoff");

        migrationBuilder.AddColumn<string>(
            name: "Title",
            table: "RewardTemplate",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<string>(
            name: "CurrencyCode",
            table: "Reward",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text",
            oldNullable: true);
    }
}
