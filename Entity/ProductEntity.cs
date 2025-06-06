namespace QLCHNT.Entity
{
    public class ProductEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Stock { get; set; }
        public string? ImageUrl { get; set; }   

        // Foreign key
        public Guid CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }

        // Navigation
        public virtual List<CartItemEntity> CartItems { get; set; }
        public virtual List<OrderDetailEntity> OrderDetails { get; set; }
    }
}
