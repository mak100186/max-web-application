using System.Text;

using ResourceName = Kindred.Rewards.Core.DomainConstants.Rn;
using ResourceNameElements = Kindred.Rewards.Core.DomainConstants.Rn.Formats.Elements;

namespace Kindred.Rewards.Core.Helpers;

/// <summary>
/// Class to generate ResourceName (aka Rn) based on patterns. No validations take place on the values provided.
/// If you want to use validated Rn(s) then use the classes inheriting from <see cref="Models.Rn.BaseRn"/>
/// </summary>
public static class SemanticResourceNameGenerator
{
    #region NestedRns
    public static string GenerateClaimRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", params string[] segments) =>
        GenerateNestedRn(namespaceValue, ResourceName.EntityTypes.Claim, schemeVersion, segments);

    public static string GenerateMarketRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", params string[] segments) =>
        GenerateNestedRn(namespaceValue, ResourceName.EntityTypes.Market, schemeVersion, segments);

    public static string GenerateVariantRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", params string[] segments) =>
        GenerateNestedRn(namespaceValue, ResourceName.EntityTypes.Variant, schemeVersion, segments);

    public static string GenerateOutcomeRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", params string[] segments) =>
        GenerateNestedRn(namespaceValue, ResourceName.EntityTypes.Outcome, schemeVersion, segments);

    private static string GenerateNestedRn(string namespaceValue, string entityType, string schemeVersion, params string[] segments)
    {
        //generate uuid if empty
        var entitySegment = string.Join(ResourceName.Separator, segments);

        //make replacements
        return ResourceName.Formats.Parent.ReplaceElements(new Dictionary<string, string>
        {
            { ResourceNameElements.Namespace, namespaceValue },
            { ResourceNameElements.EntityType, entityType },
            { ResourceNameElements.SchemeVersion, schemeVersion },
            { ResourceNameElements.EntitySegment, entitySegment },
        });
    }

    #endregion

    #region FlatRns
    public static string GenerateRewardRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", string uuid = default, string betIndex = default, string stageIndex = default) =>
       GenerateFlatRn(namespaceValue, ResourceName.EntityTypes.Reward, schemeVersion, uuid, Array.Empty<string>());

    public static string GenerateCombinationRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", string uuid = default, string betIndex = default, string stageIndex = default) =>
        GenerateFlatRn(namespaceValue, ResourceName.EntityTypes.Combination, schemeVersion, uuid, betIndex, stageIndex);

    public static string GenerateCouponRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", string uuid = default) =>
        GenerateFlatRn(namespaceValue, ResourceName.EntityTypes.Coupon, schemeVersion, uuid, Array.Empty<string>());

    public static string GenerateBetRn(string namespaceValue = ResourceName.Namespaces.Ksp, string schemeVersion = "1", string uuid = default, string index = default) =>
        GenerateFlatRn(namespaceValue, ResourceName.EntityTypes.Bet, schemeVersion, uuid, index);

    private static string GenerateFlatRn(string namespaceValue, string entityType, string schemeVersion, string uuid = default, params string[] indexes)
    {
        StringBuilder entitySegments = new();

        //generate uuid if empty
        entitySegments.Append(string.IsNullOrWhiteSpace(uuid) || !uuid.IsGuid() ? Guid.NewGuid().ToString() : uuid);

        //append index

        foreach (var index in indexes)
        {
            entitySegments.Append($"{ResourceName.Separator}{index}");
        }

        //make replacements
        return ResourceName.Formats.Parent.ReplaceElements(new Dictionary<string, string>
        {
            { ResourceNameElements.Namespace, namespaceValue },
            { ResourceNameElements.EntityType, entityType },
            { ResourceNameElements.SchemeVersion, schemeVersion },
            { ResourceNameElements.EntitySegment, entitySegments.ToString() },
        });
    }

    #endregion

    #region Private Methods
    private static string ReplaceElements(this string formatString, IDictionary<string, string> replacments)
    {
        foreach (var replacment in replacments)
        {
            formatString = formatString.Replace(replacment.Key, replacment.Value);
        }

        return formatString;
    }
    #endregion
}
