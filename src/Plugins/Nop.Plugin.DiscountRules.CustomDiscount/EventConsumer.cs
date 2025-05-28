using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.DiscountRules.CustomDiscount
{

    /// <summary>
    /// Represents the plugin event consumer
    /// </summary>
    public class EventConsumer :
        BaseAdminMenuCreatedEventConsumer
    {
        #region Fields

        private readonly IAdminMenu _adminMenu;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EventConsumer(IAdminMenu adminMenu,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPluginManager<IPlugin> pluginManager,
            IWorkContext workContext) : base(pluginManager)
        {
            _adminMenu = adminMenu;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region Utitites

        /// <summary>
        /// Checks is the current customer has rights to access this menu item
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true if access is granted, otherwise false
        /// </returns>
        protected override async Task<bool> CheckAccessAsync()
        {
            return await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_WIDGETS);
        }

        /// <summary>
        /// Gets the menu item
        /// </summary>
        /// <param name="plugin">The instance of <see cref="IPlugin"/> interface</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the instance of <see cref="AdminMenuItem"/>
        /// </returns>
        protected override async Task<AdminMenuItem> GetAdminMenuItemAsync(IPlugin plugin)
        {
            var descriptor = plugin.PluginDescriptor;

            var resource =_localizationService.GetLocaleStringResourceByName("Plugins.DiscountRules.CustomDiscount", (await _workContext.GetWorkingLanguageAsync()).Id);
            return new AdminMenuItem
            {
                SystemName = "Custom-Discount",
                Title = resource.ResourceValue,
                Visible = true,
                IconClass = "fa fa-genderless",
                Url = _adminMenu.GetMenuItemUrl("DiscountRulesCustomDiscount", "Manage")

            };

        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        protected override string PluginSystemName => "DiscountRequirement.CustomDiscount";

        /// <summary>
        /// Menu item insertion type
        /// </summary>
        protected override MenuItemInsertType InsertType => MenuItemInsertType.TryAfterThanBefore;

        /// <summary>
        /// The system name of the menu item after with need to insert the current one
        /// </summary>
        protected override string AfterMenuSystemName => "Help";

        /// <summary>
        /// The system name of the menu item before with need to insert the current one
        /// </summary>
        //protected override string BeforeMenuSystemName => "Local plugins";

        #endregion
    }
}