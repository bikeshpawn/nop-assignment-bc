
namespace Nop.Plugin.DiscountRules.CustomDiscount;

/// <summary>
/// Represents defaults for the discount requirement rule
/// </summary>
public static class DiscountRequirementDefaults
{
    /// <summary>
    /// The system name of the discount requirement rule
    /// </summary>
    public static string SystemName => "DiscountRequirement.CustomDiscount";

    /// <summary>
    /// The key of the settings to save restricted customer roles
    /// </summary>
    public static string SettingsKey => "DiscountRequirement.CustomDiscount-{0}";

    /// <summary>
    /// The HTML field prefix for discount requirements
    /// </summary>
    public static string HtmlFieldPrefix => "DiscountRulesCustomDiscount{0}";

    /// <summary>
    /// The name of discount
    /// </summary>
    public static string DiscountName => "Custom 10% Discount";
}