using System.Collections;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.All)]
public class BetTypesExtensionsTests
{
    private static IEnumerable TestSourceDataFactory
    {
        get
        {
            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.SingleLeg
                    })
                .SetName("List of 1 item of SingleBetType")
                .Returns(false);

            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.StandardMultiLeg
                    })
                .SetName("List of 1 item of StandardMultiLeg")
                .Returns(true);

            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.SystemMultiLeg
                    })
                .SetName("List of 1 item of SystemMultiLeg")
                .Returns(true);

            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.SingleLeg,
                        BetTypes.StandardMultiLeg,
                    })
                .SetName("List of 2 item of SingleBetType, StandardMultiLeg")
                .Returns(true);

            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.StandardMultiLeg,
                        BetTypes.SystemMultiLeg
                    })
                .SetName("List of 2 item of StandardMultiLeg, SystemMultiLeg")
                .Returns(true);

            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.SystemMultiLeg,
                        BetTypes.SingleLeg
                    })
                .SetName("List of 2 item of SystemMultiLeg, SingleLeg")
                .Returns(true);

            yield return new TestCaseData(
                    new List<BetTypes>
                    {
                        BetTypes.SystemMultiLeg,
                        BetTypes.SingleLeg,
                        BetTypes.StandardMultiLeg
                    })
                .SetName("List of 3 item of SystemMultiLeg, SingleLeg, StandardMultiLeg")
                .Returns(true);
        }
    }

    [TestCaseSource(typeof(BetTypesExtensionsTests), nameof(TestSourceDataFactory))]
    public bool ContainsMultiBetTypes_ReturnExpectedValue_WhenCalled(IReadOnlyCollection<BetTypes> data)
    {
        //arrange && act
        return data.ContainsMultiBetTypes();
    }
}
