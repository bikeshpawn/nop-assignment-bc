using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Migrations
{


    [NopMigration("2025/05/29 10:28:22:2551000", "Add product attribute resource string")]
    public class AddProductAttributeSearchResourceString : AutoReversingMigration
    {

        private readonly INopDataProvider _nopDataProvider;

        public AddProductAttributeSearchResourceString(INopDataProvider nopDataProvider)
        {
            _nopDataProvider = nopDataProvider;
        }

        public override void Up()
        {

            var languages = _nopDataProvider.GetTable<Language>().ToList();

            foreach (var language in languages.Where(x=>x.Published).ToList())
            {
                _nopDataProvider.InsertEntity(new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = "Admin.Catalog.Attributes.ProductAttributes.Fields.SearchAttributeName",
                    ResourceValue = "Keywords"
                });

                _nopDataProvider.InsertEntity(new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = "Admin.Catalog.Attributes.ProductAttributes.Fields.SearchAttributeName.Hint",
                    ResourceValue = "Search By Keywords"
                });


            }
        }
    }
}

