using QLCHNT.Dto.Category;
using QLCHNT.Entity;

namespace QLCHNT.Service.Category
{
    public interface ICategoryServices
    {
        Task<List<CategoryDto>> GetAll();

        Task<CategoryDto> Get(Guid Id);

        Task<Guid> Create(CategoryCreateRequest request);

        Task<Guid> Update(CategoryUpdateRequest request);

        Task<bool> Delete(Guid Id);
    }
}
