using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class ChimeraBetExtensionsTests
{
    [Test]
    [TestCase("ksp:market.1:[football:202305051700:fc_torpedo_belaz_zhodino_vs_fc_slutsk]:fc_torpedo_belaz_zhodino_clean_sheet:base",
        "football:202305051700:fc_torpedo_belaz_zhodino_vs_fc_slutsk")]
    [TestCase("ksp:market.1:[football:202304081830:1_fc_heidenheim_vs_fc_st_pauli]:both_teams_to_score:base",
        "football:202304081830:1_fc_heidenheim_vs_fc_st_pauli")]
    public void GetContestKeyFromMarket_ShouldReturnCorrectContestKey(string market, string expectedContestKey)
    {
        var actualContestKey = market.GetContestKeyFromMarket();
        Assert.AreEqual(expectedContestKey, actualContestKey);
    }
}
