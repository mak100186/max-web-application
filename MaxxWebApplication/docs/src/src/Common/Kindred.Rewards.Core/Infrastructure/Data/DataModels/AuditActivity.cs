namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public enum AuditActivity
{
    /// <summary>
    /// when bonus created in db
    /// </summary>
    BonusCreated = 0,

    /// <summary>
    /// when bonus is claimed and claim object is created in db
    /// </summary>
    ClaimCreated,

    /// <summary>
    /// when claim is revoked and bonus is made active again
    /// </summary>
    ClaimRevoked,

    /// <summary>
    /// when claim is settled for payout
    /// </summary>
    ClaimSettled,

    /// <summary>
    /// When bonus is cancelled
    /// </summary>
    BonusCancelled,

    /// <summary>
    /// When claim is unsettled and bonus is made claimed again
    /// </summary>
    ClaimUnsettled,

    /// <summary>
    /// When bonus is updated
    /// </summary>
    BonusUpdated
}
