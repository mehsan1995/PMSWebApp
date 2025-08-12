using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                return await _dbSet
                    .Where(e => EF.Property<bool>(e, "IsDeleted") == false)
                    .ToListAsync();
            }

            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<T>> FindAsyncAsNoTracking(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate);

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            return await query.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
            return entity;
        }
        public async Task<T> UpdateDetachedAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            // no SaveChanges here
            await Task.CompletedTask;
            // Detach entity to prevent stale cache
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(T entity)
        {
            if (entity is ISoftDeletable deletableEntity)
            {
                deletableEntity.IsDeleted = true;
                _dbSet.Attach(entity);
                _context.Entry(entity).Property("IsDeleted").IsModified = true;
            }
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }


}
