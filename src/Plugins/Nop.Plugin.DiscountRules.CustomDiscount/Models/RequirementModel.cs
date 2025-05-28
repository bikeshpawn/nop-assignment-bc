using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.CustomDiscount.Models;

public class RequirementModel
{

    public int DiscountId { get; set; }

    public int RequirementId { get; set; }

    [NopResourceDisplayName("Plugins.DiscountRules.CustomDiscount.Fields.NumberOfOrderRequired")]
    public int NumberOfOrderRequired { get; set; }
    

}