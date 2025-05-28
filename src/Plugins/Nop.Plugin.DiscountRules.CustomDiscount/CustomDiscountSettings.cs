using Nop.Core.Configuration;

namespace Nop.Plugin.DiscountRules.CustomDiscount;

/// <summary>
/// Represents custom discounts plugin settings
/// </summary>
public class CustomDiscountSettings : ISettings
{
    #region Properties

    public bool Enabled { get; set; }

    public decimal DiscountPercentage { get; set; }

    public int NumberOfOrderRequired { get; set; }

    #endregion
}
