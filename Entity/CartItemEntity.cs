namespace QLCHNT.Entity
{
    public class CartItemEntity
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }
        public virtual CartEntity Cart { get; set; }

        public Guid ProductId { get; set; }
        public virtual ProductEntity Product { get; set; } 
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
