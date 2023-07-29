using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.Enums;

public enum PurposeType
{
    [Display(Description = "CRM")]
    Crm,

    [Display(Description = "CUSTOMER SERVICE")]
    Customerservice,

    [Display(Description = "VIP MANAGER")]
    VipManager,

    [Display(Description = "AFFILIATE")]
    Affiliate,

    [Display(Description = "BDM")]
    Bdm
}
