namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Services;

/* TODO: Implement theses tests. These tests used to be in PromotionServiceTests.cs

[TestFixture]
[Category("Unit")]
public class PromotionServiceTests : TestBase
{
    private IRewardService service;
    private Mock<IRewardsDbContext> repository;
    private RewardDomainModel reward;
    private Mock<IRewardStrategy> UniBoostClaimStrategy;
//    private Mock<ICachingService> cache;
    private Mock<IRewardCreationStrategyFactory> strategyFactoryMock;

    [SetUp]
    public void SetUp()
    {
        this.UniBoostClaimStrategy = this.Fixture.Freeze<Mock<IRewardStrategy>>();
        this.strategyFactoryMock = this.Fixture.Freeze<Mock<IRewardCreationStrategyFactory>>();
        this.repository = this.Fixture.Freeze<Mock<IRewardsDbContext>>();
//        this.cache = this.Fixture.Freeze<Mock<ICachingService>>();
        this.service = this.Fixture.Freeze<RewardService>();

        this.reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniBoost, null);

        this.UniBoostClaimStrategy.Setup(s => s.ValidateClaim(this.reward)).Returns(true);

        this.strategyFactoryMock.Setup(f => f.GetRewardCreationStrategy(It.IsAny<RewardType>())).Returns(this.UniBoostClaimStrategy.Object);
    }

    [Test]
    public async Task ShouldCreatePromotion()
    {
        // Arrange - See SetUp()

        // Act
        await this.service.CreateAsync(this.reward);

        // Assert
        this.repository.Verify(r => r.AddAsync(It.IsAny<RewardClaimDomainModel>()), Times.Once);
    }

    [Test]
    public void ShouldNotUpdateIfPromotionNotFound()
    {
        // Arrange - 
        this.repository
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(default(RewardClaimDomainModel));

        // Act
        var error = Assert.ThrowsAsync<RewardNotFoundException>(async () => await this.service.UpdateAsync(this.reward));

        // Assert
        error.Message.Should().Be($"Could not find a reward with the provided reward key {this.reward.RewardRn}");
        this.repository.Verify(r => r.UpdateAsync(It.IsAny<RewardDomainModel>()), Times.Never);
    }

    [Test]
    public void ShouldNotPatchIfRewardNotFound()
    {
        // Arrange -
        var rewardPatchDomainModel = new RewardPatchDomainModel();

        this.repository
            .Setup(r => r.Rewards.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(default(RewardDomainModel));

        // Act
        var error = Assert.ThrowsAsync<RewardNotFoundException>(async () => await this.service.PatchAsync(reward.RewardId, rewardPatchDomainModel));

        // Assert
        error!.Message.Should().Be($"Could not find a reward with the provided reward key {reward.RewardId}");
        this.repository.Verify(r => r.Rewards.UpdateAsync(It.IsAny<RewardDomainModel>()), Times.Never);
    }

    [Test]
    public void ShouldNotPatchIfRewardIsNotLockedAndHasNotStarted()
    {
        // Arrange
        var rewardDomainModel = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Uniboost, null);

        var rewardId = rewardDomainModel.RewardId;
        var rewardPatchDomainModel = new RewardPatchDomainModel();

        rewardDomainModel.IsLocked = false; // reward has not locked
        rewardDomainModel.Terms.Restrictions.StartDateTime = DateTime.MaxValue; // reward has not started

        this.repository
            .Setup(r => r.Rewards.GetAsync(rewardId))
            .ReturnsAsync(rewardDomainModel);

        // Act
        var error = Assert.ThrowsAsync<RewardsValidationException>(async () => await this.service.PatchAsync(rewardId, rewardPatchDomainModel));

        // Assert
        error!.Message.Should().Be($"Reward [Id:{rewardDomainModel.RewardId}] is not locked and reward has not started. Therefore reward cannot be updated.");
        this.repository.Verify(r => r.Rewards.UpdateAsync(It.IsAny<RewardDomainModel>()), Times.Never);
    }

    [Test]
    [TestCase("hello", "world")]
    [TestCase("the quick brown fox jumps over the", "lazy_dog")]
    public async Task ShouldPatchWithCorrectFields(string comments, string name)
    {
        // Arrange
        var rewardDomainModel = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Uniboost, null);
        var rewardId = rewardDomainModel.RewardId;
        var rewardPatchDomainModel = new RewardPatchDomainModel() { Comments = comments, Name = name };
        rewardDomainModel.IsLocked = true; // make sure reward is locked for patch to work

        this.repository
            .Setup(r => r.Rewards.GetAsync(rewardId))
            .ReturnsAsync(rewardDomainModel);

        // Act
        await this.service.PatchAsync(rewardId, rewardPatchDomainModel);

        // Assert
        this.repository.Verify(r =>
            r.Rewards.UpdateAsync(It.Is<RewardDomainModel>(x =>
                x.Comments == comments &&
                x.Name == name)), Times.Once);
    }

    [Test]
    public async Task ShouldCancelPromotion()
    {
        // Arrange
        var rewardRn = ObjectBuilder.CreateString();
        var reason = ObjectBuilder.CreateString();

        this.repository
            .Setup(r => r.Rewards.GetAsync(rewardRn))
            .ReturnsAsync(new RewardDomainModel());

        // Act
        await this.service.CancelAsync(rewardRn, reason);

        // Assert
        this.repository.Verify(
            r => r.Rewards.UpdateAsync(
                It.Is<RewardDomainModel>(p => p.IsCancelled && p.CancellationReason == reason)),
            Times.Once);
    }

    [Test]
    public async Task ShouldReturnAllPromotions()
    {
        // Arrange
        this.repository
            .Setup(r => r.Rewards.GetRewardsByFilterAsync(It.IsAny<RewardFilterDomainModel>()))
            .ReturnsAsync(new PagedCollection<RewardDomainModel>(new List<RewardDomainModel> { new() }));

        // Act
        var result = await this.service.GetAllAsync(new(1, 100));

        // Assert
        result.Should().HaveCount(1);
    }

    [Test]
    public void ShouldReturnDefaultValuesForGivenRewardType()
    {
        // Arrange
        var strategyMock = new Mock<IRewardStrategy>();
        var optional = new List<string>();
        var required = new List<string>();

        strategyMock.Setup(x => x.OptionalParameterKeys).Returns(optional);
        strategyMock.Setup(x => x.RequiredParameterKeys).Returns(required);

        this.strategyFactoryMock.Setup(x => x.GetRewardCreationStrategy(RewardType.UniBoostReload)).Returns(strategyMock.Object);

        // Act
        var result = this.service.GetDefaults(nameof(RewardType.UniBoostReload));

        // Assert
        result.OptionalParameterKeys.Should().BeSameAs(optional);
        result.RequiredParameterKeys.Should().BeSameAs(required);
    }
}

*/
