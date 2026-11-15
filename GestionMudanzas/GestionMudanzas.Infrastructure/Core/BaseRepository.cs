using Microsoft.EntityFrameworkCore;

namespace GestionMudanzas.Infrastructure.Core
{
    public class BaseRepository<T> where T : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> DbSet;

        public BaseRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public virtual Task<List<T>> GetAllAsync()
            => DbSet.ToListAsync();

        public virtual ValueTask<T?> GetByIdAsync(int id)
            => DbSet.FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
        }
    }
}
