using AutoFixture;

using FluentAssertions;

using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Reward.Services;
using Kindred.Rewards.Plugin.Template.Services;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

using static Moq.It;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Template.Services;

[TestFixture]
[Category("Unit")]
public class TemplateCrudServiceTests : TestBase
{
    private ITemplateCancellationService _cancellationService;
    private ITemplateCrudService _service;
    private Mock<ILogger<TemplateCrudService>> _logger;
    private Mock<IKafkaProducerManager> _producer;
    private RewardsDbContextMock _context;

    [SetUp]
    public void SetUp()
    {
        _logger = new();

        var bluePrintConfigurationModel = new BlueprintsConfigurationModel
        {
            DefaultTemplate = string.Empty,
            LockedTemplates = new()
        };

        var blueprintOptions = Options.Create(bluePrintConfigurationModel);
        var timeService = new TimeService();
        _producer = new();

        _context = new(new DbContextOptionsBuilder<RewardsDbContextMock>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

        _service = new TemplateCrudService(_logger.Object, _context,
            blueprintOptions, timeService, Mapper, _producer.Object);
        _cancellationService = Fixture.Freeze<ITemplateCancellationService>();
    }


    [Test]
    public async Task ShouldCreate()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();

        // Act
        var result = await _service.CreateAsync(template);

        // Assert
        result.Should().BeEquivalentTo(new { Id = 1, template.Comments, template.Title, template.TemplateKey, template.CreatedBy, template.UpdatedBy });
    }

    [Test]
    public void ShouldNotCreateIfAlreadyExist()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();

        _service.CreateAsync(template);

        // Act
        var error = Assert.ThrowsAsync<RewardsValidationException>(() => _service.CreateAsync(template));

        // Assert
        error.Message.Should().Be($"Promotion template exist with template key {template.TemplateKey}");
    }


    [Test]
    public async Task ShouldReturnAllTemplates()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();

        // Act
        await _service.CreateAsync(template);

        var result = await _service.GetAllAsync(new(), new());

        // Assert
        result.ItemCount.Should().Be(1);
    }

    [Test]
    public async Task ShouldNotDeleteIfNotFound()
    {
        // Arrange
        var templateKey = ObjectBuilder.CreateString();

        // Act
        await _service.DeleteAsync(templateKey);

        // Assert
        _logger.Verify(r => r.Log(LogLevel.Warning, 0, IsAny<IsAnyType>(), null, IsAny<Func<IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Test]
    public async Task ShouldDeleteIfFound()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();

        await _service.CreateAsync(template);
        var beforeDelete = await _service.GetAsync(template.TemplateKey, false);

        // Act
        await _service.DeleteAsync(template.TemplateKey);
        var afterDelete = await _service.GetAsync(template.TemplateKey, false);

        // Assert
        beforeDelete.Enabled.Should().BeTrue();
        afterDelete.Enabled.Should().BeFalse();
    }

    [Test]
    public async Task ShouldReturnTemplateWithPromotions()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();
        template.Rewards = new List<RewardDomainModel>
        {
            Fixture
                .Build<RewardDomainModel>()
                .With(x => x.RewardId, Guid.NewGuid().ToString())
                .Create()
        };

        await _service.CreateAsync(template);

        // Act
        var result = await _service.GetAsync(template.TemplateKey, null);

        // Assert
        result.ShouldNotBeNull();
        result.Rewards.Should().HaveCount(1);
    }


    [Test]
    public async Task ShouldNotReturnTemplateIfNotFound()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();
        await _service.CreateAsync(template);

        var templateKey = Guid.NewGuid().ToString();
        // Act
        var error = Assert.ThrowsAsync<PromotionTemplateNotFoundException>(() =>
            _service.GetAsync(templateKey, null));

        // Assert
        error.Message.Should().Be($"Cannot find promotion template with template key {templateKey}");
    }

    [Test]
    public async Task ShouldMapTemplateToPromotions()
    {
        // Arrange
        var template = PromotionTemplateBuilder.Create();
        template.Rewards = new List<RewardDomainModel>();

        var rewardRns = ObjectBuilder.CreateManyStrings();

        foreach (var rewardRn in rewardRns)
        {
            template.Rewards.Add(new()
            {
                RewardId = rewardRn,
                Terms = Fixture.Build<RewardTerms>().Create()
            });
        }

        await _service.CreateAsync(template);

        // Act
        await _service.UpdateMappingAsync(template.TemplateKey, rewardRns, null);

        // Assert
        _producer.Verify(r =>
                r.SendAsync(DomainConstants.TopicProfileTemplateUpdates, IsAny<object>(), IsAny<string>(), IsAny<string>(), IsAny<Dictionary<string, string>>()),
            Times.Exactly(2));

        _producer.Reset();
    }

    [Test]
    public void ShouldNotMapTemplateToPromotionsIfTemplateNotFound()
    {
        // Arrange
        var templateKey = ObjectBuilder.CreateString();
        var rewardRns = ObjectBuilder.CreateManyStrings();

        // Act
        var error = Assert.ThrowsAsync<PromotionTemplateNotFoundException>(() => _service.UpdateMappingAsync(templateKey, rewardRns, null));

        // Assert
        error.Message.Should().Be($"Cannot find promotion template with template key {templateKey}");
    }


    [Test]
    public void ShouldNotMapTemplateToPromotionsIfPromotionNotFound()
    {
        // Arrange
        var templateKey = ObjectBuilder.CreateString();

        var template = PromotionTemplateBuilder.Create();
        template.TemplateKey = templateKey;

        var rewardRn = ObjectBuilder.CreateString();

        _service.CreateAsync(template);

        // Act
        var error = Assert.ThrowsAsync<PromotionsNotFoundException>(() =>
            _service.UpdateMappingAsync(templateKey, new() { rewardRn }, null));

        // Assert
        error.Message.Should().Be($"Could not find promotions with reward keys {rewardRn}");
    }


    [Test]
    [Ignore("KCR787")]
    public async Task ShouldRemoveCancelledPromotionFromTemplateMapping()
    {
        // Arrange
        var rewardId = ObjectBuilder.CreateString();
        var customerIds = ObjectBuilder.CreateManyStrings();
        var templates = LoopUtilities.CreateMany(() => PromotionTemplateBuilder.Create(template =>
        {
            template.Rewards = new List<RewardDomainModel>
            {
                RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward =>
                {
                    reward.RewardId = rewardId;
                })
            };
        }));

        foreach (var template in templates)
        {
            for (var i = 0; i < template.Rewards.Count; i++)
            {
                var promotion = template.Rewards[i];
                promotion.RewardId = rewardId;
                promotion.CustomerId = customerIds[i];
            }

            await _service.CreateAsync(template);
        }

        // Act
        await _cancellationService.RemoveCancelledPromotionAsync(rewardId);

        // Assert
        foreach (var template in templates)
        {
            var expectedRewardRns = template.Rewards.Where(p => p.RewardId != rewardId).Select(p => p.RewardId);

            var doesExist = await _service.GetAsync(template.TemplateKey, false);

            // TODO: Check what could be done here
            //this.repository.Verify(
            //    r => r.RewardTemplates.SetPromotionToPromotionTemplateAsync(
            //        template.TemplateKey,
            //        expectedRewardRns),
            //    Times.Once);
        }

        //this.cache.Verify(c => c.Clear(It.IsAny<IReadOnlyCollection<string>>()), Times.Exactly(3));
    }
}
