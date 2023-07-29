using FluentAssertions;

using Kindred.Rewards.Plugin.Base.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Base;

[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.All)]
public class HealthProbeTests
{

    [Test]
    public void HealthProbe_ReturnsUnhealthyStatus_WhenErrorsArePresent()
    {
        //arrange
        var healthProbe = new HealthProbe();

        healthProbe.AddError("source 1", "error message 1");
        healthProbe.AddError("source 1", "error message 2");
        healthProbe.AddError("source 1", "error message 3");
        healthProbe.AddError("source 2", "error message 1");
        healthProbe.AddError("source 2", "error message 2");
        healthProbe.AddError("source 3", "error message 1");
        healthProbe.AddError("source 3", "error message 2");

        //act
        var status = healthProbe.GetStatus();

        //assert
        status.Status.Should().Be(HealthStatus.Unhealthy);
        status.Description.Should().ContainAll("The following errors occurred:", "source 3 : error message 1 | error message 2", "source 2 : error message 1 | error message 2", "source 1 : error message 1 | error message 2 | error message 3");
        status.Data.Count.Should().Be(3);
    }

    [Test]
    public void HealthProbe_ReturnsHealthyStatus_WhenNoErrorsArePresent()
    {
        //arrange
        var healthProbe = new HealthProbe();

        //act
        var status = healthProbe.GetStatus();

        //assert
        status.Status.Should().Be(HealthStatus.Healthy);
        status.Description.Should().BeNullOrWhiteSpace();
    }
}
