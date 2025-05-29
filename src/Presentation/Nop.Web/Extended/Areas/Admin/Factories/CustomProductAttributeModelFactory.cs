using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the product attribute model factory implementation
/// </summary>
public partial class CustomProductAttributeModelFactory : ProductAttributeModelFactory
{

    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;

    #endregion

    #region Ctor

    public CustomProductAttributeModelFactory(ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IProductAttributeService productAttributeService,
        IProductService productService):base(
        localizationService,
        localizedModelFactory,
        productAttributeService,
        productService
            )
    {
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _productAttributeService = productAttributeService;
        _productService = productService;
    }

    #endregion


    #region Methods

    public override async Task<ProductAttributeListModel> PrepareProductAttributeListModelAsync(ProductAttributeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get product attributes
        var productAttributes = await _productAttributeService
            .GetAllProductAttributesExtendedAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, searchKeyword:searchModel.SearchAttributeName);

        //prepare list model
        var model = new ProductAttributeListModel().PrepareToGrid(searchModel, productAttributes, () =>
        {
            //fill in model values from the entity
            return productAttributes.Select(attribute => attribute.ToModel<ProductAttributeModel>());

        });

        return model;
    }

    #endregion
}
