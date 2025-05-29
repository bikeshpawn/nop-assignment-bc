using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;


namespace Nop.Web.Infrastructure
{
    public class ExtendedNopStartup : INopStartup
    {
        public int Order => 2005;

        public void Configure(IApplicationBuilder application)
        {
           
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProductAttributeModelFactory, CustomProductAttributeModelFactory>();
           
        }

    }
}
