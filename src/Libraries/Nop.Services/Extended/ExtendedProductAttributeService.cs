using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core;

namespace Nop.Services.Catalog;
public partial class ProductAttributeService : IProductAttributeService
{
    /// <summary>
    /// Gets all product attributes
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attributes
    /// </returns>
    public virtual async Task<IPagedList<ProductAttribute>> GetAllProductAttributesExtendedAsync(int pageIndex = 0,
        int pageSize = int.MaxValue, string searchKeyword=null)
    {
        var productAttributes = await _productAttributeRepository.GetAllPagedAsync(query =>
        {
            if (!string.IsNullOrWhiteSpace(searchKeyword))
                query = query.Where(c => c.Name.Contains(searchKeyword));
            return from pa in query
                   orderby pa.Name
                   select pa;
        }, pageIndex, pageSize);

        return productAttributes;
    }
}
