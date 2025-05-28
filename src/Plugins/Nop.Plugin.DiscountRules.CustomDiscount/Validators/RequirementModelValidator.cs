using FluentValidation;
using Nop.Plugin.DiscountRules.CustomDiscount.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.DiscountRules.CustomDiscount.Validators;

/// <summary>
/// Represents an <see cref="RequirementModel"/> validator.
/// </summary>
public class RequirementModelValidator : BaseNopValidator<RequirementModel>
{
    public RequirementModelValidator(ILocalizationService localizationService)
    {
        RuleFor(model => model.DiscountId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.DiscountId.Required"));
    }
}