using Microsoft.EntityFrameworkCore;
using QLCHNT.DATA;
using System.Linq.Expressions;

namespace QLCHNT.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context; // Database 
        private readonly DbSet<TEntity> _dbSet; // Database => Table< Task, Category>

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>(); 
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            var result = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task CreateListAsync(List<TEntity> entities)
        {
            // Danh sách đối tương tạo
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _dbSet.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
            if (result == null)
            {
                throw new Exception("Không tìm thấy đối tượng");
            }
            // Xoa
            _dbSet.Remove(result);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteListAsync(List<Guid> ids)
        {
            //Danh sách đối tượng cần xóa, đã tìm theo danh sách id
            var result = await _dbSet.Where(x => ids.Contains(EF.Property<Guid>(x, "Id"))).ToListAsync();
            if (result.Count == 0)
            {
                throw new Exception("Không tìm thấy đối tượng");
            }
            _dbSet.RemoveRange(result);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var result = await _dbSet.Where(predicate).ToListAsync();
            if (result.Count == 0)
            {
                throw new Exception("Không có dữ liệu");
            }
            return result;
        }

        public async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var result = await _dbSet.FirstOrDefaultAsync(predicate);
            return result;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            var result = await _dbSet.ToListAsync(); // Lấy danh sách
            if (result.Count == 0)
            {
                throw new Exception("Không có dữ liệu");
            }
            return result;
        }

        public async Task<TEntity> GetAsync(Guid id)
        {
            // Lấy 1 đối tương theo Id
            var result = await _dbSet.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
            if (result == null)
            {
                throw new Exception("Không tìm thấy đối tượng");
            }
            return result;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }
    }
}
