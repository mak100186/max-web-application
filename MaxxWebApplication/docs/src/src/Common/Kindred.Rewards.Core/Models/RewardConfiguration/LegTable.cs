using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;

using Newtonsoft.Json;

namespace Kindred.Rewards.Core.Models.RewardConfiguration;

public class LegTable
{
    public LegTable(IDictionary<int, decimal> table, IReadOnlyCollection<BetTypes> betTypes)
    {
        Initialize(table, betTypes);
    }

    public ICollection<LegDefinition> LegDefinitions { get; private set; }

    private void Initialize(IDictionary<int, decimal> table, IReadOnlyCollection<BetTypes> betTypes)
    {
        //add to list
        List<LegDefinition> legDefinitions = new();

        foreach ((var leg, var value) in table)
        {
            legDefinitions.Add(new(leg, leg, value));
        }

        //order
        var orderedLegDefinitions = legDefinitions.OrderBy(x => x.LegRange.Start).ToArray();

        //infer ranges
        for (var i = 0; i < orderedLegDefinitions.Length - 1; i++)
        {
            var current = orderedLegDefinitions[i];

            if (i + 1 >= orderedLegDefinitions.Length)
            {
                continue;
            }

            var next = orderedLegDefinitions[i + 1];

            current.LegRange.End = next.LegRange.Start - 1;
        }

        orderedLegDefinitions.Last().LegRange.End = DomainConstants.MaxNumberOfLegsInMulti;

        LegDefinitions = orderedLegDefinitions;

        //validate
        Validate();
    }

    private void Validate()
    {
        for (var i = 0; i < LegDefinitions.Count - 1; i++)
        {
            var current = LegDefinitions.ElementAt(i);
            var next = LegDefinitions.ElementAt(i + 1);

            if (current.LegRange.ContainsRange(next.LegRange))
            {
                throw new($"Ranges overlap: {current} - {next}");
            }
        }
    }

    public LegDefinition GetLegDefinition(int legCount)
    {
        return LegDefinitions.FirstOrDefault(x => x.ContainsLeg(legCount));
    }

    public static LegTable GetLegTable(IDictionary<string, string> parameters, BetTypes betType)
    {
        try
        {
            var table = JsonConvert.DeserializeObject<Dictionary<int, decimal>>(
                parameters.GetValue<string>(RewardParameterKey.LegTable));

            if (table == null || !table.Any())
            {
                throw new($"Leg definitions for the following bet type is expected: [{betType}]");
            }

            switch (betType)
            {
                case BetTypes.SingleLeg:

                    if (!table.ContainsKey(1))
                    {
                        throw new("For BetTypes.SingleLeg, a leg definition for 1 leg was expected");
                    }

                    break;
                case BetTypes.StandardMultiLeg:
                case BetTypes.SystemMultiLeg:

                    if (!table.HasAnyKeys(DomainConstants.MultiBetValidLegs))
                    {
                        throw new($"For BetTypes.{betType}, at least one leg definition for 2 and above legs was expected");
                    }

                    break;
                default:
                    throw new($"Unknown {betType} passed for validation");
            }

            return new(table, new List<BetTypes> { betType });
        }
        catch (Exception e)
        {
            throw new($"{RewardParameterKey.LegTable} is invalid. See inner exception", e);
        }
    }
}
