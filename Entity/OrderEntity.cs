using QLCHNT.Const;

namespace QLCHNT.Entity
{
    public class OrderEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public DateTime OrderDate { get; set; }
        public Enums.Status Status { get; set; }

        public virtual List<OrderDetailEntity> OrderDetails { get; set; }
    }
}
