using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuickService.LoanRepayment.Infrastructure.Data.Repositories
{
    public class BaseEfRepository<T, Tkey> : IRepository<T, Tkey> where T : class
    {
        protected readonly AppDbContext DbContext; // Beware. Leaky Abstraction Todo: Figure out a way to refactor leaky abstraction on EfContext

        public BaseEfRepository(AppDbContext context)
        {
            DbContext = context;
        }

        public virtual async Task<T> AddItem(T tEntity)
        {
            try
            {
                //using var db = new AppDbContext();
                await DbContext.Set<T>().AddAsync(tEntity);
                return (await DbContext.SaveChangesAsync()) > 0 ? tEntity : null;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> AddItems(IEnumerable<T> tEntities)
        {
            try
            {
                //using var db = new AppDbContext();
                await DbContext.Set<T>().AddRangeAsync(tEntities);
                return (await DbContext.SaveChangesAsync()) > 0 ? tEntities : null;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public virtual async Task<T> GetItem(Tkey id)
        {
            try
            {
                //using var db = new AppDbContext();
                return await DbContext.Set<T>().FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetItems()
        {
            try
            {
                //using var db = new AppDbContext();
                return await DbContext.Set<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetItems(Func<T, bool> predicate)
        {
            try
            {
                //using var db = new AppDbContext();
                return await Task.Run(() => DbContext.Set<T>().Where(predicate).ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> RemoveItem(Tkey id)
        {
            try
            {
                //using (var db = new AppDbContext())

                var item = await GetItem(id);
                if (item == null)
                    return false;
                DbContext.Set<T>().Remove(item);
                return await DbContext.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<T> UpdateItem(Tkey id, T updatedEntity)
        {
            try
            {
                //using (var db = new AppDbContext())
                //{
                var item = await GetItem(id);
                if (item == null)
                    throw new KeyNotFoundException($"Item with key {id} not found");
                DbContext.Entry<T>(updatedEntity).State = EntityState.Modified; // .CurrentValues..SetValues(updatedItem);
                var updated = await DbContext.SaveChangesAsync() == 1;
                return updated ? item : null;
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}