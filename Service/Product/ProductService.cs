using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using QLCHNT.Dto.Order;
using QLCHNT.Dto.Product;
using QLCHNT.Entity;
using QLCHNT.Repository;

namespace QLCHNT.Service.Product
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ProductEntity> _rpProduct;

        public ProductService(
            IMapper mapper, IRepository<ProductEntity> repository)
        {
            _mapper = mapper;
            _rpProduct = repository;

        }

        public async Task<Guid> Create(ProductCreateRequest request)
        {

            var productExit = await _rpProduct.AsQueryable().AnyAsync(t => t.Name == request.Name);
            if (productExit)
            {
                throw new Exception("Sản phẩm đã tồn tại");
            }
            var entity = _mapper.Map<ProductEntity>(request);

            await _rpProduct.CreateAsync(entity);
            return entity.Id;

        }

        public async Task<bool> Delete(Guid Id)
        {
            var deleteExit = await _rpProduct.GetAsync(Id); // ✅ Đợi xong trước
            if (deleteExit == null)
                throw new Exception("Sản phẩm không tồn tại");

            await _rpProduct.DeleteAsync(Id); // ✅ Gọi tiếp sau khi xong
            return true;
        }

        public Task<PagedResult<ProductDto>> GetPagedProductsAsync(ProductGetPageingRequest request)
        {
            var query = _rpProduct.AsQueryable();

            // 🔍 Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                request.SearchText = request.SearchText.ToLower();
                // use contains to search: id, phone, address, productName
                query = query.Where(o => o.Name.ToString().ToLower().Contains(request.SearchText))
                    .Include(o => o.Category);;
            }
            
            // Tổng và phân trang
            var total = query.Count();
            if (request.PageIndex.HasValue && request.PageSize.HasValue)
            {
                query = query.Skip((request.PageIndex.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
            }

            var mappedItems = _mapper.Map<List<ProductDto>>(query.ToList());

            return Task.FromResult( new PagedResult<ProductDto>
            {
                TotalItems = total,
                Items = mappedItems
            });
        }

        public async Task<string?> UploadProductImageAsync(IFormFile imageFile, Guid productId)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new Exception("File không hợp lệ");

            // Tìm product theo Id
            var product = await _rpProduct.GetAsync(productId);
            if (product == null)
                throw new Exception("Không tìm thấy sản phẩm");

            // Upload lên Cloudinary
            var account = new Account("dmmukvwvi", "595564171248616", "8U0ZRAJe75pAOwPsRpxIAsMG38g");
            var cloudinary = new Cloudinary(account);

            await using var stream = imageFile.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imageFile.FileName, stream),
                Folder = "productImage"
            };

            var result =  cloudinary.Upload(uploadParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Upload lỗi: {result.Error?.Message}");

            // Cập nhật URL vào sản phẩm
            product.ImageUrl = result.SecureUri.ToString();
            await _rpProduct.UpdateAsync(product);

            return product.ImageUrl;
        }


        public async Task<Guid> Update(ProductUpdate request)
        {
            var prudateExist = await _rpProduct.GetAsync(request.Id);

            // Mapper sang UserEntity
            _mapper.Map(request, prudateExist);

            await _rpProduct.UpdateAsync(prudateExist);

            // Cập nhaatj thanhf coong
            return prudateExist.Id;
        }

       public async Task<List<ProductDto>>GetAll()
        {
            var result = await _rpProduct.AsQueryable().Include(x=> x.Category).ToListAsync();
            return _mapper.Map<List<ProductDto>>(result);

        }

       public async Task<ProductDto>Get(Guid Id)
       {
            var productExit = await _rpProduct.AsQueryable().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == Id );
            if (productExit == null)
            {
                throw new Exception("Không tìm thấy sản phẩm");
            }
            return _mapper.Map<ProductDto>(productExit);
        }
    }
}
