using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web.Framework.Infrastructure
{
    public class ExtendedStartup : INopStartup
    {
        public int Order => 101;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddExtendedViews();
        }
        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
