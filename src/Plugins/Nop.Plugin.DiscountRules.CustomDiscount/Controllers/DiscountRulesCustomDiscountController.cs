using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.CustomDiscount.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.DiscountRules.CustomDiscount.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class DiscountRulesCustomDiscountController : BasePluginController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IDiscountService _discountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly CustomDiscountSettings _discountSettings;
    protected readonly IStoreContext _storeContext;
    protected readonly INotificationService _notificationService;


    #endregion

    #region Ctor

    public DiscountRulesCustomDiscountController(ICustomerService customerService,
        IDiscountService discountService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        ISettingService settingService,
        CustomDiscountSettings discountSettings,
        IStoreContext storeContext,
        INotificationService notificationService)
    {
        _customerService = customerService;
        _discountService = discountService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _discountSettings = discountSettings;
        _storeContext = storeContext;
        _notificationService = notificationService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get errors message from model state
    /// </summary>
    /// <param name="modelState">Model state</param>
    /// <returns>Errors message</returns>
    protected IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
    {
        return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
    }

    #endregion


    #region Methods
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Manage()
    {
        var model = new ConfigurationModel
        {
            Enabled = _discountSettings.Enabled,
            DiscountPercentage = _discountSettings.DiscountPercentage
        };
        return View("~/Plugins/DiscountRules.CustomDiscount/Views/Manage.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Manage(ConfigurationModel model)
    {
        await _settingService.SetSettingAsync($"{nameof(CustomDiscountSettings)}.{nameof(CustomDiscountSettings.Enabled)}", model.Enabled);
        await _settingService.SetSettingAsync($"{nameof(CustomDiscountSettings)}.{nameof(CustomDiscountSettings.DiscountPercentage)}", model.DiscountPercentage);


        var discount = (await _discountService.GetAllDiscountsAsync(discountName:DiscountRequirementDefaults.DiscountName,isActive:null))?.FirstOrDefault();
        if (discount != null)
        {
            discount.DiscountPercentage = model.DiscountPercentage;
            discount.IsActive = model.Enabled;
            await _discountService.UpdateDiscountAsync(discount);
        }

        //settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return RedirectToAction("Manage");
    }

    #endregion

    #region Configure Discount Requirments

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public async Task<IActionResult> Configure(int discountId, int? discountRequirementId)
    {
        //load the discount
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
                       ?? throw new ArgumentException("Discount could not be loaded");

        //check whether the discount requirement exists
        if (discountRequirementId.HasValue && await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId.Value) is null)
            return Content("Failed to load requirement.");

        var model = new RequirementModel
        {
            RequirementId = discountRequirementId ?? 0,
            DiscountId = discountId,
            NumberOfOrderRequired = _discountSettings.NumberOfOrderRequired
        };

        //set the HTML field prefix
        ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(DiscountRequirementDefaults.HtmlFieldPrefix, discountRequirementId ?? 0);

        return View("~/Plugins/DiscountRules.CustomDiscount/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public async Task<IActionResult> Configure(RequirementModel model)
    {
        if (ModelState.IsValid)
        {
            //load the discount
            var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId);
            if (discount == null)
                return NotFound(new { Errors = new[] { "Discount could not be loaded" } });

            //get the discount requirement
            var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(model.RequirementId);

            //the discount requirement does not exist, so create a new one
            if (discountRequirement == null)
            {
                discountRequirement = new DiscountRequirement
                {
                    DiscountId = discount.Id,
                    DiscountRequirementRuleSystemName = DiscountRequirementDefaults.SystemName

                };

                await _discountService.InsertDiscountRequirementAsync(discountRequirement);
            }

            await _settingService.SetSettingAsync($"{nameof(CustomDiscountSettings)}.{nameof(CustomDiscountSettings.NumberOfOrderRequired)}", model.NumberOfOrderRequired);

            return Ok(new { NewRequirementId = discountRequirement.Id });
        }

        return Ok(new { Errors = GetErrorsFromModelState(ModelState) });
    }

    #endregion

}