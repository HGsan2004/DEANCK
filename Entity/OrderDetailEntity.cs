namespace QLCHNT.Entity
{
    public class OrderDetailEntity
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public virtual OrderEntity Order { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public virtual ProductEntity Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }  
    }
}
