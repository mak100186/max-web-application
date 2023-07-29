namespace Kindred.Rewards.Core.Mapping.Converters;

public interface IConverter<TSource, TDestination>
{
    TDestination Convert(TSource sourceObject);

    (IEnumerable<TDestination> converted, IEnumerable<TSource> invalid) Convert(IEnumerable<TSource> sourceObjects);
}
