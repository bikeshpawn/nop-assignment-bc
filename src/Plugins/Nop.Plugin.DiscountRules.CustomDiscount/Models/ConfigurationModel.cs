using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.CustomDiscount.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{

    [NopResourceDisplayName("Plugins.DiscountRules.CustomDiscount.Fields.Enabled")]
    public bool Enabled { get; set; }
    public bool Enabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.DiscountRules.CustomDiscount.Fields.DiscountPercentage")]
    public decimal DiscountPercentage { get; set; }
    public bool DiscountPercentage_OverrideForStore { get; set; }

}
