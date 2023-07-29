namespace Kindred.Rewards.Core.Abstractions;

public interface IResourceName
{
    string OriginalLiteral { get; set; }
    string Namespace { get; set; }
    string EntityScheme { get; set; }
    string EntityType { get; set; }
    int? SchemeVersion { get; set; }
}
