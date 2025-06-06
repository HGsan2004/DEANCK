namespace QLCHNT.Entity
{
    public class CartEntity
    {
        public Guid Id { get; set; }

        // Foreign key
        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public virtual List<CartItemEntity> CartItems { get; set; }
    } 
}
