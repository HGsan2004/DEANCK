namespace QLCHNT.Dto.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Stock { get; set; }
        public string? ImageUrl { get; set; }  

        // Foreign key
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
