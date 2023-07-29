using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#pragma warning disable IDE0161 // Convert to file-scoped namespace
namespace Kindred.Rewards.Infrastructure.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reward",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RewardType = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: true),
                    Purpose = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: true),
                    SubPurpose = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BackendTitle = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    RewardRules = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PromoId = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TermsJson = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    CountryCode = table.Column<string>(type: "character varying(2)", unicode: false, maxLength: 2, nullable: true),
                    Jurisdiction = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Brand = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CustomerFacingName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CancellationReason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SourceInstanceId = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reward", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RewardKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    CouponRef = table.Column<string>(type: "character varying(255)", unicode: false, maxLength: 255, nullable: false),
                    BetRef = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", unicode: false, maxLength: 20, nullable: false),
                    TermsJson = table.Column<string>(type: "character varying(5000)", unicode: false, maxLength: 5000, nullable: false),
                    IntervalId = table.Column<long>(type: "bigint", nullable: false),
                    UsageId = table.Column<int>(type: "integer", nullable: false),
                    InstanceId = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    RewardPayoutAmount = table.Column<decimal>(type: "numeric(16,2)", nullable: true),
                    PromoId = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RewardType = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    BetOutcomeStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(3)", unicode: false, maxLength: 3, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardClaim", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    InsertDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RewardTemplateCustomer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PromotionTemplateKey = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    InsertDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTemplateCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", maxLength: 128, nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", unicode: false, maxLength: 2000, nullable: true),
                    DisplayStyle = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditReward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RewardId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Activity = table.Column<string>(type: "character varying(30)", unicode: false, maxLength: 30, nullable: false),
                    InsertDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditReward_Reward_RewardId",
                        column: x => x.RewardId,
                        principalTable: "Reward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RewardTemplateReward",
                columns: table => new
                {
                    RewardTemplateId = table.Column<int>(type: "integer", nullable: false),
                    RewardId = table.Column<string>(type: "character varying(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTemplateReward", x => new { x.RewardTemplateId, x.RewardId });
                    table.ForeignKey(
                        name: "FK_RewardTemplateReward_Reward_RewardId",
                        column: x => x.RewardId,
                        principalTable: "Reward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RewardTemplateReward_RewardTemplate_RewardTemplateId",
                        column: x => x.RewardTemplateId,
                        principalTable: "RewardTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RewardTags",
                columns: table => new
                {
                    RewardId = table.Column<string>(type: "character varying(128)", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardTags", x => new { x.RewardId, x.TagId });
                    table.ForeignKey(
                        name: "FK_RewardTags_Reward_RewardId",
                        column: x => x.RewardId,
                        principalTable: "Reward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RewardTags_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditReward_RewardId",
                table: "AuditReward",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_Reward_CustomerExpiry",
                table: "Reward",
                columns: new[] { "CustomerId", "ExpiryDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Reward_CustomerName",
                table: "Reward",
                columns: new[] { "CustomerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardClaim_InstanceId",
                table: "RewardClaim",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardClaim_Usage",
                table: "RewardClaim",
                columns: new[] { "RewardKey", "CustomerId", "IntervalId", "UsageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardTags_RewardId",
                table: "RewardTags",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardTags_TagId",
                table: "RewardTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardTemplate_Key",
                table: "RewardTemplate",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardTemplateCustomer_CustomerId",
                table: "RewardTemplateCustomer",
                columns: new[] { "CustomerId", "PromotionTemplateKey" });

            migrationBuilder.CreateIndex(
                name: "IX_RewardTemplateCustomer_RewardTemplateKey",
                table: "RewardTemplateCustomer",
                columns: new[] { "PromotionTemplateKey", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_RewardTemplateReward_RewardKey",
                table: "RewardTemplateReward",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardTemplateReward_RewardTemplateKey",
                table: "RewardTemplateReward",
                column: "RewardTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Name",
                table: "Tag",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditReward");

            migrationBuilder.DropTable(
                name: "RewardClaim");

            migrationBuilder.DropTable(
                name: "RewardTags");

            migrationBuilder.DropTable(
                name: "RewardTemplateCustomer");

            migrationBuilder.DropTable(
                name: "RewardTemplateReward");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Reward");

            migrationBuilder.DropTable(
                name: "RewardTemplate");
        }
    }
}
#pragma warning restore IDE0161 // Convert to file-scoped namespace
