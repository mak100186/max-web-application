using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class RenameUpdatedDateToUpdatedOnAndInsertDateToCreatedOn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UpdateDate",
            table: "Tag",
            newName: "UpdatedOn");

        migrationBuilder.RenameColumn(
            name: "InsertDate",
            table: "Tag",
            newName: "CreatedOn");

        migrationBuilder.RenameColumn(
            name: "InsertDate",
            table: "RewardTemplateCustomer",
            newName: "CreatedOn");

        migrationBuilder.RenameColumn(
            name: "UpdateDate",
            table: "RewardTemplate",
            newName: "UpdatedOn");

        migrationBuilder.RenameColumn(
            name: "InsertDate",
            table: "RewardTemplate",
            newName: "CreatedOn");

        migrationBuilder.RenameColumn(
            name: "UpdateDate",
            table: "RewardClaim",
            newName: "UpdatedOn");

        migrationBuilder.RenameColumn(
            name: "InsertDate",
            table: "RewardClaim",
            newName: "CreatedOn");

        migrationBuilder.RenameColumn(
            name: "UpdateDate",
            table: "Reward",
            newName: "UpdatedOn");

        migrationBuilder.RenameColumn(
            name: "InsertDate",
            table: "Reward",
            newName: "CreatedOn");

        migrationBuilder.RenameColumn(
            name: "InsertDate",
            table: "AuditReward",
            newName: "CreatedOn");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "UpdatedOn",
            table: "Tag",
            newName: "UpdateDate");

        migrationBuilder.RenameColumn(
            name: "CreatedOn",
            table: "Tag",
            newName: "InsertDate");

        migrationBuilder.RenameColumn(
            name: "CreatedOn",
            table: "RewardTemplateCustomer",
            newName: "InsertDate");

        migrationBuilder.RenameColumn(
            name: "UpdatedOn",
            table: "RewardTemplate",
            newName: "UpdateDate");

        migrationBuilder.RenameColumn(
            name: "CreatedOn",
            table: "RewardTemplate",
            newName: "InsertDate");

        migrationBuilder.RenameColumn(
            name: "UpdatedOn",
            table: "RewardClaim",
            newName: "UpdateDate");

        migrationBuilder.RenameColumn(
            name: "CreatedOn",
            table: "RewardClaim",
            newName: "InsertDate");

        migrationBuilder.RenameColumn(
            name: "UpdatedOn",
            table: "Reward",
            newName: "UpdateDate");

        migrationBuilder.RenameColumn(
            name: "CreatedOn",
            table: "Reward",
            newName: "InsertDate");

        migrationBuilder.RenameColumn(
            name: "CreatedOn",
            table: "AuditReward",
            newName: "InsertDate");
    }
}
