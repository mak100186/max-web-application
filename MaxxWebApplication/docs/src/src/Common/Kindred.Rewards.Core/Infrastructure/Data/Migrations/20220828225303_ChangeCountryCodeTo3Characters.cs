using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations;

public partial class ChangeCountryCodeTo3Characters : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "CountryCode",
            table: "Reward",
            type: "character varying(3)",
            unicode: false,
            maxLength: 3,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(2)",
            oldUnicode: false,
            oldMaxLength: 2,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "CountryCode",
            table: "Reward",
            type: "character varying(2)",
            unicode: false,
            maxLength: 2,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(3)",
            oldUnicode: false,
            oldMaxLength: 3,
            oldNullable: true);
    }
}
