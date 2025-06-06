using QLCHNT.Dto.Cart;

namespace QLCHNT.Service.Cart
{
    public interface ICartService
    {
        Task<CartDto> AddToCartAsync(AddCartItemDto request);
        Task<CartDto?> UpdateCartItemAsync(UpdateCartItem request);
        Task<bool> Delete(Guid id);
        Task<List<CartDto>> GetAll(); 
        Task<CartDto> GetByUserIdAsync(Guid userId);
    }
}
