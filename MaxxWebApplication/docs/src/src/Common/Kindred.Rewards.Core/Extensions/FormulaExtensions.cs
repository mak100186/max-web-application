using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Extensions;

public static class FormulaExtensions
{
    public static IReadOnlyCollection<string> GetAllFormulae()
    {
        return GetStandardFormulae().Concat(GetSystemFormulae()).ToList();
    }

    public static IReadOnlyCollection<string> GetFormulaeForBetTypes(IReadOnlyCollection<BetTypes> betTypes)
    {
        List<string> formulae = new();

        foreach (var betType in betTypes)
        {
            switch (betType)
            {
                case BetTypes.SingleLeg:
                    formulae.AddRange(GetSingleFormulae());
                    break;
                case BetTypes.StandardMultiLeg:
                    formulae.AddRange(GetStandardFormulae());
                    break;
                case BetTypes.SystemMultiLeg:
                    formulae.AddRange(GetSystemFormulae());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return formulae;
    }

    public static IReadOnlyCollection<string> GetSingleFormulae()
    {
        return new List<string>
        {
            "singles"
        };
    }

    public static IReadOnlyCollection<string> GetStandardFormulae()
    {
        return new List<string>
        {
            "doubles",
            "eighteenfolds",
            "eightfolds",
            "elevenfolds",
            "fifteenfolds",
            "fivefolds",
            "fourfolds",
            "fourteenfolds",
            "ninefolds",
            "nineteenfolds",
            "sevenfolds",
            "seventeenfolds",
            "singles",
            "sixfolds",
            "sixteenfolds",
            "tenfolds",
            "thirteenfolds",
            "trebles",
            "twelvefolds",
            "twentyfolds"
        };
    }

    public static IReadOnlyCollection<string> GetSystemFormulae()
    {
        return new List<string>
        {
            "canadian",
            "goliath",
            "heinz",
            "lucky15",
            "lucky31",
            "lucky63",
            "patent",
            "superheinz",
            "trixie",
            "yankee"
        };
    }

    //also known as ComboMulti
    public static bool IsStandardMulti(this string formula)
    {
        return GetStandardFormulae().Contains(formula.ToLowerInvariant());
    }

    //also known as FormulaMulti
    public static bool IsSystemMulti(this string formula)
    {
        return GetSystemFormulae().Contains(formula.ToLowerInvariant());
    }

    public static IReadOnlyCollection<BetTypes> GetApplicableToBetTypes(
        this IReadOnlyCollection<string> formulae,
        int minStages = 1, int maxStages = 1,
        int minCombinations = 1, int maxCombinations = 1)
    {
        List<BetTypes> applicableTo = new();

        if (formulae.IsNullOrEmpty())
        {
            //by default, SingleLeg is allowed
            applicableTo.AddIfNotPresent(BetTypes.SingleLeg);

            if (maxStages >= 2)
            {
                //StandardMulti is only allowed based on maxStage count
                applicableTo.AddIfNotPresent(BetTypes.StandardMultiLeg);
            }
        }

        foreach (var formula in formulae)
        {
            switch (formula.ToLowerInvariant())
            {
                case "singles":
                    //this takes care of a multi made up of singles
                    if (maxStages >= 2)
                    {
                        applicableTo.AddIfNotPresent(BetTypes.StandardMultiLeg);
                    }

                    //this takes care of single leg bet with singles formula
                    if (minStages == 1)
                    {
                        applicableTo.AddIfNotPresent(BetTypes.SingleLeg);
                    }
                    break;
                default:
                    //this takes care of all other formulae, no need to include combinations here (for now)
                    if (formula.IsStandardMulti() && minCombinations >= 1 && maxCombinations >= 1)
                    {
                        applicableTo.AddIfNotPresent(BetTypes.StandardMultiLeg);
                    }

                    if (formula.IsSystemMulti() && minCombinations >= 1 && maxCombinations >= 1)
                    {
                        applicableTo.AddIfNotPresent(BetTypes.SystemMultiLeg);
                    }

                    break;
            }
        }

        return applicableTo.AsReadOnly();
    }


}
