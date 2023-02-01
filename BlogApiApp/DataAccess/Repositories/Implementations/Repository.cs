using DataAccess.DataContext;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.ApiModels.ResponseDTO;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
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
            var res = query.FirstOrDefault();
            return res;

        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
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
            return query.ToList();
        }

        protected async Task<PagedList<T>> GetPageAsync(IQueryable<T> query, int pageNumber, int pageSize/*, string? includeProperties = null*/)
        {
            return await PagedList<T>.CreateAsync(query, pageNumber, pageSize);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
    }
}
