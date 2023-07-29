using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.WebApi.Enums;

public enum PurposeType
{
    [Display(Description = "CRM")]
    Crm,

    [Display(Description = "CUSTOMER SERVICE")]
    Customerservice,

    [Display(Description = "VIP MANAGER")]
    Vipmanager,

    [Display(Description = "AFFILIATE")]
    Affiliate,

    [Display(Description = "BDM")]
    Bdm
}
