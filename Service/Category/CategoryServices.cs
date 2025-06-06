using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLCHNT.DATA;
using QLCHNT.Dto.Category;
using QLCHNT.Dto.Product;
using QLCHNT.Entity;
using QLCHNT.Repository;

namespace QLCHNT.Service.Category
{

    public class CategoryServices : ICategoryServices
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CategoryEntity> _rpcategory;

        public CategoryServices(
            IMapper mapper, IRepository<CategoryEntity> repository)
        {
            _mapper = mapper;
            _rpcategory = repository;

        }

        public async Task<Guid> Create(CategoryCreateRequest request)
        {
            var categoryExist = await _rpcategory.AsQueryable().AnyAsync(c => c.Name == request.Name);

            if (categoryExist)
            {
                throw new Exception("Sản phẩm đã tồn tại");
            }

            var entity = _mapper.Map<CategoryEntity>(request);

            // Thêm entity vào CSDL
            await _rpcategory.CreateAsync(entity);

            // Trả về Id của danh mục vừa thêm
            return entity.Id;
        }

        public async Task<bool>Delete(Guid Id)
        {
            var deleteExit = await _rpcategory.GetAsync(Id); // ✅ Đợi xong trước
            if (deleteExit == null)
                throw new Exception("Sản phẩm không tồn tại");

            await _rpcategory.DeleteAsync(Id); // ✅ Gọi tiếp sau khi xong
            return true;
        }

        public async Task<CategoryDto> Get(Guid Id)
        {
            // Lấy danh mục theo Id
            var categoryExist = await _rpcategory.GetAsync(Id);
            // Danh mục tìm thấy
            return _mapper.Map<CategoryDto>(categoryExist);
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            // lấy tất cả danh sách danh mục
            var categories = await _rpcategory.GetAllAsync();
            if (categories == null)
            {
                throw new Exception("Không có danh mục nào."); // xử lý ở Service
            }
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<Guid> Update(CategoryUpdateRequest request)
        {
            var catedateExist = await _rpcategory.GetAsync(request.Id);

            // Mapper sang UserEntity
            _mapper.Map(request, catedateExist);

            await _rpcategory.UpdateAsync(catedateExist);

            // Cập nhaatj thanhf coong
            return catedateExist.Id;
        }
    }
        
}
