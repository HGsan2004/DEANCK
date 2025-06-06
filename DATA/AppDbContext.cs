using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using QLCHNT.Entity;

namespace QLCHNT.DATA
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet cho mỗi thực thể
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<CartEntity> Carts { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderDetailEntity> OrderDetails { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseLazyLoadingProxies();
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Định nghĩa khóa chính và quan hệ nếu cần

            // CartItem: Many-to-One (Cart)
            modelBuilder.Entity<CartItemEntity>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem: Many-to-One (Product)
            modelBuilder.Entity<CartItemEntity>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderDetail: Many-to-One (Order)
            modelBuilder.Entity<OrderDetailEntity>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderDetail: Many-to-One (Product)
            modelBuilder.Entity<OrderDetailEntity>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product - Category: N-1
            modelBuilder.Entity<ProductEntity>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Cart: 1-N
            //modelBuilder.Entity<CartEntity>()
               // .HasOne(c => c.User)
                //.WithMany(u => u.Carts)
                //.HasForeignKey(c => c.UserId)
                //.OnDelete(DeleteBehavior.Cascade);

            // User - Order: 1-N
           // modelBuilder.Entity<OrderEntity>()
                //.HasOne(o => o.User)
                //.WithMany(u => u.Orders)
                //.HasForeignKey(o => o.UserId)
                //.OnDelete(DeleteBehavior.Cascade);

            // Indexes, Constraints, hoặc Seed thêm tùy bạn cần
        }
    }
}
