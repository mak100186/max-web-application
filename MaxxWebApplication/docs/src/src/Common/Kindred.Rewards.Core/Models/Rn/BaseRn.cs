using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Abstractions;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class BaseRn : IResourceName
{
    public string OriginalLiteral { get; set; }
    public string Namespace { get; set; }
    public string EntityScheme { get; set; }
    public string EntityType { get; set; }
    public int? SchemeVersion { get; set; }
    public List<string> EntitySegments { get; set; } = new();

    public BaseRn(string @namespace, string entityType, int schemeVersion)
    {
        OriginalLiteral = null;

        Namespace = @namespace;

        EntityType = entityType;
        SchemeVersion = schemeVersion;

        EntityScheme = $"{entityType}.{schemeVersion}";
    }

    public BaseRn(string rnString)
    {
        OriginalLiteral = rnString;

        var elements = GetRnElements(rnString);

        var errors = Validate(elements);
        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        Namespace = elements[0];
        EntityScheme = elements[1];

        var schemeElements = EntityScheme.Split(".");

        EntityType = schemeElements[0];
        SchemeVersion = schemeElements[1].ToInt();

        if (elements.Length > 2)
        {
            for (var i = 2; i < elements.Length; i++)
            {
                EntitySegments.Add(elements[i]);
            }
        }

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }
    }

    private string[] GetRnElements(string rnString)
    {
        var trimmedRn = rnString.Trim().Replace("[", "").Replace("]", "");
        return trimmedRn.Split(DomainConstants.Rn.Separator);
    }

    private List<string> Validate(string[] elements)
    {
        List<string> errors = new();


        if (elements.Length <= 2)
        {
            errors.Add("Rn cannot have 2 or less segments");
        }

        if (!elements[0].IsValidNamespace())
        {
            errors.Add("Invalid namespace");
        }

        var entityScheme = elements[1];
        if (string.IsNullOrWhiteSpace(entityScheme) || !entityScheme.Contains('.'))
        {
            errors.Add("Invalid entity scheme");
        }
        else
        {
            var entitySchemeELements = entityScheme.Split('.');
            if (entitySchemeELements.Length != 2)
            {
                errors.Add("Invalid entity scheme");
            }

            if (!entitySchemeELements[1].IsNumeric())
            {
                errors.Add("Invalid scheme version");
            }
        }

        return errors;
    }
}
