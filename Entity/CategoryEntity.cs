namespace QLCHNT.Entity
{
    public class CategoryEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property
        public virtual List<ProductEntity> Products { get; set; }
    }
}
