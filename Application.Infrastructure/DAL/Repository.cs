using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext dbContext;
        
        public Repository(ApplicationDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.Table = dbContext.Set<T>();
        }
        public DbSet<T> Table { get; set; }

        public async Task<bool> Add(T entity)
        {
            Table.Add(entity);
            return await SaveAsync();
        }

        public async  Task<bool> Update(T entity)
        {
            Table.Update(entity);
            return await SaveAsync();
        }

        public async Task<bool> Delete(T entity)
        {
            Table.Remove(entity);
            return await SaveAsync();
        }

        public IQueryable<T> All()
        {
            return Table;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> where)
        {
            return Table.Where(where);
        }

        public IQueryable<T> OrderBy<TKey>(Expression<Func<T, TKey>> orderBy, bool isDesc)
        {
            if (isDesc)
                return Table.OrderByDescending(orderBy);
            return Table.OrderBy(orderBy);
        }


        private async Task<bool> SaveAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                // TODO: Log Exceptions
                return false;
            }
        }
    }
}