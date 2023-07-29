using System.Linq.Expressions;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using LinqKit;

namespace Kindred.Rewards.Core.Infrastructure.Data.Extensions;
public static class SearchPredicateBuilder
{
    public static Expression<Func<Tag, bool>> BuildSpecificTagsPredicate(IEnumerable<string> tagNames)
    {
        var predicate = PredicateBuilder.New<Tag>(false);

        foreach (var tagName in tagNames)
        {
            var lowerCaseTagName = tagName.ToLowerInvariant();
            predicate = predicate.Or(tag => tag.Name.ToLower().Equals(lowerCaseTagName));
        }

        return predicate;
    }
}
