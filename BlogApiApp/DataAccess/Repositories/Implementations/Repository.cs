using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.ApiModels.ResponseDTO;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _appContext;
        internal DbSet<T> dbSet;

        public Repository(AppDbContext appContext)
        {
            _appContext = appContext;
            this.dbSet = _appContext.Set<T>();
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query = tracked ? dbSet.AsTracking() : dbSet.AsNoTracking();
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return await query.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return await query.ToListAsync();
        }

        protected async Task<PaginatedList<T>> GetPageAsync(IQueryable<T> query, int pageNumber, int pageSize/*, string? includeProperties = null*/)
        {
            return await PaginatedList<T>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
