﻿using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product attribute search model
/// </summary>
public partial record ProductAttributeSearchModel : BaseSearchModel
{
    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.Fields.SearchAttributeName")]
    public string SearchAttributeName { get; set; }
}