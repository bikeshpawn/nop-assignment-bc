using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Nop.Data;

namespace Nop.Web.Framework.Infrastructure.Extensions
{
    public static class CustomServiceCollectionExtensions
    {
        public static void AddExtendedViews(this IServiceCollection services)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ExtendedViewExpander());
            });
        }
    }
}
