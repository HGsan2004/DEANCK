using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using QLCHNT.Dto.Cart;
using QLCHNT.Entity;
using QLCHNT.Repository;

namespace QLCHNT.Service.Cart
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CartEntity> _rpcart;
        private readonly IRepository<CartItemEntity> _cartItem;
        private readonly IRepository<ProductEntity> _rpProduct;
        private readonly IHttpContextAccessor _httpContextAccessor; // Lấy thông tin người dùng từ token

        public CartService(
            IMapper mapper, IRepository<CartEntity> repository,
            IRepository<CartItemEntity> repository1,
            IRepository<ProductEntity> repository2,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _rpcart = repository;
            _cartItem = repository1;
            _httpContextAccessor = httpContextAccessor;
            _rpProduct = repository2;
        }

        public async Task<CartDto> AddToCartAsync(AddCartItemDto request)
        {
            //Kiểm tra ng dùng userid có cart chưa 
            //Nếu chưa tiến hành tạo mới cart nêu có r tiến hành thêm item cho cart 
            //map từ Addcaritemtdto sang cartitemtentity  gọi _cartitem để thêm        
            var cart = await _rpcart.FirstOrDefault(c => c.UserId == request.UserId);
            if (cart == null)
            {
                cart = new CartEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId                                       
                };

                await _rpcart.CreateAsync(cart);
            }

            // Kiểm tra nếu sản phẩm đã có trong giỏ
            var existingItem = await _cartItem.FirstOrDefault(ci => ci.CartId == cart.Id 
                                                              && ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                await _cartItem.UpdateAsync(existingItem);
            }
            else
            {
                var product = await _rpProduct.FirstOrDefault(p=>p.Id ==request.ProductId);
                if (product == null)
                {
                    throw new Exception("San pham khong ton tai");
                }
                var cartItem = _mapper.Map<CartItemEntity>(request);

                cartItem.Price = product.Price;
                cartItem.CartId = cart.Id;
                await _cartItem.CreateAsync(cartItem);
            }

            cart = await _rpcart.AsQueryable()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);
            
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> Delete(Guid id)
        {
            var deleteExit = await _rpcart.GetAsync(id); // ✅ Đợi xong trước
            if (deleteExit == null)
                throw new Exception("Sản phẩm không tồn tại");

            await _rpcart.DeleteAsync(id); // ✅ Gọi tiếp sau khi xong
            return true;
        }
        
        public async Task<List<CartDto>> GetAll()
        {
            // lấy tất cả danh sách danh mục
            var cartExit = await _rpcart.AsQueryable()
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .ToListAsync();
            return _mapper.Map<List<CartDto>>(cartExit);
        }

        public async Task<CartDto> GetByUserIdAsync(Guid userId)
        {
            var cartExit = _rpcart.AsQueryable()
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == userId);
            if (cartExit == null)
            {
                // Nếu chưa có giỏ thì tạo mới
                cartExit = new CartEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CartItems = new List<CartItemEntity>()
                };
                await _rpcart.CreateAsync(cartExit);
            }
            return _mapper.Map<CartDto>(cartExit);
        }
        
        public async Task<CartDto?> UpdateCartItemAsync(UpdateCartItem request)
        {
            
            var existingItem = await _cartItem.FirstOrDefault(ci => ci.Id == request.Id);
            if (existingItem == null)
            {
                throw new Exception("không tìm thấy sản phẩm trong giỏ hàng");
            }
            existingItem.Quantity = request.Quantity;
            await _cartItem.UpdateAsync(existingItem);

          var cart = await _rpcart.AsQueryable()
              .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
              .Include(c => c.User)
              .FirstOrDefaultAsync(c => c.Id == existingItem.CartId);
            
            return _mapper.Map<CartDto>(cart);
        }
    }
    
}
