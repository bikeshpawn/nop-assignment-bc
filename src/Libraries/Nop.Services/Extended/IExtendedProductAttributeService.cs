using Nop.Core.Domain.Catalog;
using Nop.Core;

namespace Nop.Services.Catalog;
public partial interface IProductAttributeService
{
    Task<IPagedList<ProductAttribute>> GetAllProductAttributesExtendedAsync(int pageIndex = 0, int pageSize = int.MaxValue, string searchKeyword=null);
}
