using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class ContestKey
{
    public string Type { get; set; }
    public string DateTime { get; set; }
    public string Identifier { get; set; }

    public ContestKey(string type, string dateTime, string identifier)
    {
        Type = type;
        DateTime = dateTime;
        Identifier = identifier;

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }
    }

    public ContestKey(string contestKeyLiteral)
    {
        var trimmedRn = contestKeyLiteral.Trim().Replace("[", "").Replace("]", "");
        var elements = trimmedRn.Split(DomainConstants.Rn.Separator);

        if (elements.IsNullOrEmpty() || elements.Length != 3)
        {
            throw new AggregateException("Contest Key must have 3 segments");
        }

        Type = elements[0];
        DateTime = elements[1];
        Identifier = elements[2];

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(Type))
        {
            errors.Add("Contest Type cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(DateTime))
        {
            errors.Add("Contest DateTime cannot be empty");
        }
        else
        {
            if (!DateTime.IsDate())
            {
                errors.Add("Invalid Contest DateTime");
            }
        }

        if (string.IsNullOrWhiteSpace(Identifier))
        {
            errors.Add("Contest Identifier cannot be empty");
        }

        return errors;
    }

    public override string ToString() => $"[{Type}:{DateTime}:{Identifier}]";
}
