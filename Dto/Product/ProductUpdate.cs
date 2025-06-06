namespace QLCHNT.Dto.Product
{
    public class ProductUpdate
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Stock { get; set; }

        // Foreign key
        public Guid CategoryId { get; set; }
    }
}
