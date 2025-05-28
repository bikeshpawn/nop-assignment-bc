using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;

namespace Nop.Plugin.DiscountRules.CustomDiscount;

public class CustomDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly ICustomerService _customerService;
    protected readonly IDiscountService _discountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IWebHelper _webHelper;
    protected readonly IOrderService _orderService;
    protected readonly CustomDiscountSettings _customDiscontSettings;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CustomDiscountRequirementRule(IActionContextAccessor actionContextAccessor,
        IDiscountService discountService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        IWebHelper webHelper,
        IOrderService orderService,
        CustomDiscountSettings customDiscontSettings,
        IWorkContext workContext)
    {
        _actionContextAccessor = actionContextAccessor;
        _customerService = customerService;
        _discountService = discountService;
        _localizationService = localizationService;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _webHelper = webHelper;
        _orderService = orderService;
        _customDiscontSettings = customDiscontSettings;
        _workContext = workContext;
    }

    #endregion

    #region Methods


    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/DiscountRulesCustomDiscount/Manage";
    }


    /// <summary>
    /// Check discount requirement
    /// </summary>
    /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        //invalid by default
        var result = new DiscountRequirementValidationResult();

        if (request.Customer == null)
            return result;

        //result is valid if the customer has previous order count 3 or greater than 3
        var customerOrdersCount = (await _orderService.SearchOrdersAsync(customerId: request.Customer.Id,getOnlyTotalCount:true)).TotalCount;
        result.IsValid = customerOrdersCount >= _customDiscontSettings.NumberOfOrderRequired;

        return result;
    }

    /// <summary>
    /// Get URL for rule configuration
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
    /// <returns>URL</returns>
    public string GetConfigurationUrl(int discountId, int? discountRequirementId)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        return urlHelper.Action("Configure", "DiscountRulesCustomDiscount",
            new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {

        //insert default setting value
        var setting = new CustomDiscountSettings
        {
            Enabled = true,
            DiscountPercentage = 10,
            NumberOfOrderRequired=3
        };

        await _settingService.SaveSettingAsync(setting);

        //create new discount if doe snot exist, we are using nopcommerce discount module so accessing by name
        var discountCounts = await _discountService.GetAllDiscountsAsync(discountName:DiscountRequirementDefaults.DiscountName, isActive:null);
        if (!discountCounts.Any())
        {
            

            var discount = new Discount
            {
                IsActive = true,
                Name = DiscountRequirementDefaults.DiscountName,
                UsePercentage = true,
                DiscountPercentage = 10,
                RequiresCouponCode = false,
                DiscountType = DiscountType.AssignedToOrderTotal,
                DiscountLimitation = DiscountLimitationType.Unlimited,
               
            };

            await _discountService.InsertDiscountAsync(discount);


            //check for default discount requirement group
            var defaultRequirementGroup = _localizationService.GetLocaleStringResourceByName("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup", (await _workContext.GetWorkingLanguageAsync()).Id);

            var defaultDiscountRequirementGroup = (await _discountService.GetAllDiscountRequirementsAsync()).Where(x => x.IsGroup && x.DiscountRequirementRuleSystemName == defaultRequirementGroup.ResourceValue.ToString()).FirstOrDefault();
            if (defaultDiscountRequirementGroup == null)
            {
                defaultDiscountRequirementGroup = new DiscountRequirement();
                defaultDiscountRequirementGroup.DiscountId = discount.Id;
                defaultDiscountRequirementGroup.IsGroup = true;
                defaultDiscountRequirementGroup.InteractionTypeId = (int)RequirementGroupInteractionType.And;
                defaultDiscountRequirementGroup.DiscountRequirementRuleSystemName = defaultRequirementGroup.ResourceValue;

                await _discountService.InsertDiscountRequirementAsync(defaultDiscountRequirementGroup);

            }

            //create discount requirement for above discount
            var discountRequirement = new DiscountRequirement
            {
                DiscountId = discount.Id,
                IsGroup = false,
                DiscountRequirementRuleSystemName = DiscountRequirementDefaults.SystemName,
                ParentId = defaultDiscountRequirementGroup.Id
            };

            await _discountService.InsertDiscountRequirementAsync(discountRequirement);
        }

        

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.DiscountRules.CustomDiscount.Fields.NumberOfOrderRequired"] = "No. Of Orders",
            ["Plugins.DiscountRules.CustomDiscount.Fields.NumberOfOrderRequired.hint"] = "Specify no. of orders required to apply this discount",
            ["Plugins.DiscountRules.CustomDiscount.Fields.Enabled"] = "Enable",
            ["Plugins.DiscountRules.CustomDiscount.Field.Enabled.hint"] = "Check to enable this plugin",
            ["Plugins.DiscountRules.CustomDiscount.Fields.DiscountPercentage"] = "Discount Percentage",
            ["Plugins.DiscountRules.CustomDiscount.Fields.DiscountPercentage.hint"] = "Specify Discount Percentage",
            ["Plugins.DiscountRules.CustomDiscount.Settings"] = "Custom Discount Settings",
            ["Plugins.DiscountRules.CustomDiscount"] = "Custom Discount",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {

        var customDiscounts = await _discountService.GetAllDiscountsAsync(discountName: DiscountRequirementDefaults.DiscountName);

        foreach(var discount in customDiscounts){

            var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
            .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountRequirementDefaults.SystemName);
            foreach (var discountRequirement in discountRequirements)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
            }

            await _discountService.DeleteDiscountAsync(discount);

        }

        //settings
        await _settingService.DeleteSettingAsync<CustomDiscountSettings>();

        //locales
        await _localizationService.DeleteLocaleResourceAsync("Plugins.DiscountRules.CustomDiscount");
        ;
        await base.UninstallAsync();
    }

    #endregion
}