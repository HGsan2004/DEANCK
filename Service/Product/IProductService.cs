using QLCHNT.Dto.Order;
using QLCHNT.Dto.Product;
using QLCHNT.Dto.User;
using QLCHNT.Entity;

namespace QLCHNT.Service.Product
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAll();
        Task<ProductDto> Get(Guid Id);

        Task<Guid> Create(ProductCreateRequest request);

        Task<Guid> Update(ProductUpdate request);

        Task<bool> Delete(Guid Id);      
        Task<PagedResult<ProductDto>> GetPagedProductsAsync(ProductGetPageingRequest request);
        
        Task<string?> UploadProductImageAsync(IFormFile imageFile, Guid productId);
    }
}
