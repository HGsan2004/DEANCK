namespace QLCHNT.Dto.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new();
    }
}
