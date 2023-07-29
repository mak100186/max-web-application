using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Rewards.Repositories.Migrations;

public partial class NewCustomerRewardTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsSystemGenerated",
            table: "Reward",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "CustomerRewards",
            columns: table => new
            {
                CustomerId = table.Column<string>(type: "text", nullable: false),
                RewardId = table.Column<string>(type: "character varying(128)", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerRewards", x => new { x.CustomerId, x.RewardId });
                table.ForeignKey(
                    name: "FK_CustomerReward_Reward",
                    column: x => x.RewardId,
                    principalTable: "Reward",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CustomerRewards_CustomerId",
            table: "CustomerRewards",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_CustomerRewards_RewardId",
            table: "CustomerRewards",
            column: "RewardId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CustomerRewards");

        migrationBuilder.DropColumn(
            name: "IsSystemGenerated",
            table: "Reward");
    }
}
