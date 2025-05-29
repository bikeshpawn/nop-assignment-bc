using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Web.Framework
{
    public class ExtendedViewExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            viewLocations = new string[] { 
                "/Extended/Areas/{2}/Views/{1}/{0}.cshtml",
                "/Extended/Areas/{2}/Views/Shared/{0}.cshtml"
            }.Concat(viewLocations);
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            
        }
    }
}
