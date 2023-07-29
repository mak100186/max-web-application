﻿// <auto-generated />

#nullable disable

using Kindred.Rewards.Core.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kindred.Rewards.Infrastructure.Data.Migrations
{
    [DbContext(typeof(RewardsDbContext))]
    [Migration("20221205035304_RemoveRewardCategory")]
    partial class RemoveRewardCategory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.AuditReward", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Activity")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<DateTime>("CreatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<string>("RewardId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.HasIndex("RewardId")
                        .HasDatabaseName("IX_AuditReward_RewardId");

                    b.ToTable("AuditReward", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.Reward", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("BackendTitle")
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)");

                    b.Property<string>("Brand")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("CancellationReason")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ContestStatus")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("CountryCode")
                        .HasMaxLength(3)
                        .IsUnicode(false)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomerFacingName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("CustomerId")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("Description")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<DateTime>("ExpiryDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("boolean");

                    b.Property<string>("Jurisdiction")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("NamingConvention")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Purpose")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("RewardRules")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("RewardType")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("SourceInstanceId")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("StartDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("State")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("SubPurpose")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TermsJson")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId", "ExpiryDateTime")
                        .HasDatabaseName("IX_Reward_CustomerExpiry");

                    b.HasIndex("CustomerId", "Name")
                        .IsUnique()
                        .HasDatabaseName("IX_Reward_CustomerName");

                    b.ToTable("Reward", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BetJson")
                        .HasColumnType("text");

                    b.Property<string>("BetOutcomeStatus")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("BetRn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CouponRn")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("CreatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<string>("CurrencyCode")
                        .HasMaxLength(3)
                        .IsUnicode(false)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("InstanceId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<long>("IntervalId")
                        .HasColumnType("bigint");

                    b.Property<string>("NamingConvention")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<decimal?>("RewardPayoutAmount")
                        .HasColumnType("numeric(16,2)");

                    b.Property<string>("RewardRn")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("RewardType")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("TermsJson")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .IsUnicode(false)
                        .HasColumnType("character varying(5000)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<int>("UsageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("InstanceId")
                        .HasDatabaseName("IX_RewardClaim_InstanceId");

                    b.HasIndex("RewardRn", "CustomerId", "IntervalId", "UsageId")
                        .IsUnique()
                        .HasDatabaseName("IX_RewardClaim_Usage");

                    b.ToTable("RewardClaim", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTag", b =>
                {
                    b.Property<string>("RewardId")
                        .HasColumnType("character varying(128)")
                        .HasColumnName("RewardId");

                    b.Property<int>("TagId")
                        .HasColumnType("integer")
                        .HasColumnName("TagId");

                    b.HasKey("RewardId", "TagId");

                    b.HasIndex("RewardId")
                        .HasDatabaseName("IX_RewardTags_RewardId");

                    b.HasIndex("TagId")
                        .HasDatabaseName("IX_RewardTags_TagId");

                    b.ToTable("RewardTags", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(300)
                        .IsUnicode(true)
                        .HasColumnType("character varying(300)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasDatabaseName("IX_RewardTemplate_Key");

                    b.ToTable("RewardTemplate", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTemplateCustomer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("PromotionTemplateKey")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId", "PromotionTemplateKey")
                        .HasDatabaseName("IX_RewardTemplateCustomer_CustomerId");

                    b.HasIndex("PromotionTemplateKey", "CustomerId")
                        .HasDatabaseName("IX_RewardTemplateCustomer_RewardTemplateKey");

                    b.ToTable("RewardTemplateCustomer", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTemplateReward", b =>
                {
                    b.Property<int>("RewardTemplateId")
                        .HasColumnType("integer")
                        .HasColumnName("RewardTemplateId");

                    b.Property<string>("RewardRn")
                        .HasColumnType("character varying(128)")
                        .HasColumnName("RewardId");

                    b.HasKey("RewardTemplateId", "RewardRn");

                    b.HasIndex("RewardRn")
                        .HasDatabaseName("IX_RewardTemplateReward_RewardRn");

                    b.HasIndex("RewardTemplateId")
                        .HasDatabaseName("IX_RewardTemplateReward_RewardTemplateKey");

                    b.ToTable("RewardTemplateReward", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .IsUnicode(false)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("DisplayStyle")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasPrecision(6)
                        .HasColumnType("timestamp(6) with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("IX_Tag_Name");

                    b.ToTable("Tag", (string)null);
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.AuditReward", b =>
                {
                    b.HasOne("Kindred.Rewards.Rewards.Repositories.DataModels.Reward", null)
                        .WithMany("Audits")
                        .HasForeignKey("RewardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTag", b =>
                {
                    b.HasOne("Kindred.Rewards.Rewards.Repositories.DataModels.Reward", "Reward")
                        .WithMany("RewardTags")
                        .HasForeignKey("RewardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kindred.Rewards.Rewards.Repositories.DataModels.Tag", "Tag")
                        .WithMany("RewardTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reward");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTemplateReward", b =>
                {
                    b.HasOne("Kindred.Rewards.Rewards.Repositories.DataModels.Reward", "Reward")
                        .WithMany("RewardTemplateReward")
                        .HasForeignKey("RewardRn")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTemplate", "RewardTemplate")
                        .WithMany("RewardTemplateReward")
                        .HasForeignKey("RewardTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reward");

                    b.Navigation("RewardTemplate");
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.Reward", b =>
                {
                    b.Navigation("Audits");

                    b.Navigation("RewardTags");

                    b.Navigation("RewardTemplateReward");
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.RewardTemplate", b =>
                {
                    b.Navigation("RewardTemplateReward");
                });

            modelBuilder.Entity("Kindred.Rewards.Rewards.Repositories.DataModels.Tag", b =>
                {
                    b.Navigation("RewardTags");
                });
#pragma warning restore 612, 618
        }
    }
}
