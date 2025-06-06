using System.Linq.Expressions;

namespace QLCHNT.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> AsQueryable(); // Hành động truy vấn tự do, gồm nhiều thao tác, tự truy vấn
                                           // Tái sử dụng hành động
                                           // Create:  hành động tạo 1 đối tượng
        Task<TEntity> CreateAsync(TEntity entity);

        // CreateListAsync: Tạo nhiều đối tượng cùng lúc
        Task CreateListAsync(List<TEntity> entities);

        // Update: hành động cập nhật đối tượng
        Task<TEntity> UpdateAsync(TEntity entity);

        // Delete: hành động xóa 1 đối tượng
        Task DeleteAsync(Guid id);

        // DeleteListAsync: Xóa nhiều đối tượng cùng lúc
        Task DeleteListAsync(List<Guid> ids);

        // Get, GetAll
        // Get: lấy 1 đối tượng theo Id
        Task<TEntity> GetAsync(Guid id);

        //GetAll: lấy tất cả dữ liệu thuộc đối tượng T
        Task<List<TEntity>> GetAllAsync();

        // Lấy 1 đối tượng theo điều kiện 
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        // Lấy danh sách đối tượng theo điều kiện
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        
    }
}
