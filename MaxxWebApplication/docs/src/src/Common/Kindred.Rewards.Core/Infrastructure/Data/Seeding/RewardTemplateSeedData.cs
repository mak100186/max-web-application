using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;

namespace Kindred.Rewards.Core.Infrastructure.Data.Seeding;
internal class RewardTemplateSeedData
{
    private readonly ModelBuilder _modelBuilder;

    public RewardTemplateSeedData(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }

    public void Seed()
    {
        _modelBuilder.Entity<RewardTemplate>().HasData(
            new RewardTemplate
            {
                Id = 1,
                Enabled = true,
                Title = "UB_NEW",
                Comments = "UB_NEW",
                Key = "UB_NEW",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 2,
                Enabled = true,
                Title = "UB_STANDARD",
                Comments = "UB_STANDARD",
                Key = "UB_STANDARD",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 3,
                Enabled = true,
                Title = "UB_VALUED_SMALL",
                Comments = "UB_VALUED_SMALL",
                Key = "UB_VALUED_SMALL",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 4,
                Enabled = true,
                Title = "UB_VALUED_MEDIUM",
                Comments = "UB_VALUED_MEDIUM",
                Key = "UB_VALUED_MEDIUM",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 5,
                Enabled = true,
                Title = "UB_VALUED_LARGE",
                Comments = "UB_VALUED_LARGE",
                Key = "UB_VALUED_LARGE",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 6,
                Enabled = true,
                Title = "UB_VIP_SMALL",
                Comments = "UB_VIP_SMALL",
                Key = "UB_VIP_SMALL",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 7,
                Enabled = true,
                Title = "UB_VIP_MEDIUM",
                Comments = "UB_VIP_MEDIUM",
                Key = "UB_VIP_MEDIUM",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 8,
                Enabled = true,
                Title = "UB_VIP_LARGE",
                Comments = "UB_VIP_LARGE",
                Key = "UB_VIP_LARGE",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 9,
                Enabled = true,
                Title = "NO_REWARDS",
                Comments = "NO_REWARDS",
                Key = "NO_REWARDS",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new RewardTemplate
            {
                Id = 10,
                Enabled = true,
                Title = "UB_NO_REWARDS_FALLBACK",
                Comments = "UB_NO_REWARDS_FALLBACK",
                Key = "UB_NO_REWARDS_FALLBACK",
                CreatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                UpdatedOn = new(2023, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            }
            );
    }
}
