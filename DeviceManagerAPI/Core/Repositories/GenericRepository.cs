using System.Linq.Expressions;
using DeviceApp.Data;
using DevicesApp.Core.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DevicesApp.Core.Repositories;

public class GenericRepository<T>(AppDbContext context, ILogger logger) : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<T> dbSet = context.Set<T>();
    protected readonly ILogger _logger = logger;

    public virtual async Task<IEnumerable<T>> GetAll()
    {
        try
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetAll method error", typeof(T).Name);
            return Enumerable.Empty<T>();
        }
    }

    public virtual async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includes)
    {
        try
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetAll with includes method error", typeof(T).Name);
            return Enumerable.Empty<T>();
        }
    }

    public virtual async Task<T?> GetById(Guid id)
    {
        try
        {
            return await dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetById method error", typeof(T).Name);
            return null;
        }
    }

    public virtual async Task<T?> GetById(Guid id, params Expression<Func<T, object>>[] includes)
    {
        try
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetById with includes method error", typeof(T).Name);
            return null;
        }
    }

    public virtual async Task<T?> Find(Expression<Func<T, bool>> criteria)
    {
        try
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Find method error", typeof(T).Name);
            return null;
        }
    }

    public virtual async Task<T?> Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
    {
        try
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.SingleOrDefaultAsync(criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Find with includes method error", typeof(T).Name);
            return null;
        }
    }

    public virtual async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> criteria)
    {
        try
        {
            return await dbSet.AsNoTracking().Where(criteria).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} FindAll method error", typeof(T).Name);
            return Enumerable.Empty<T>();
        }
    }

    public virtual async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
    {
        try
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await query.Where(criteria).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} FindAll with includes method error", typeof(T).Name);
            return Enumerable.Empty<T>();
        }
    }

    public virtual async Task<bool> Add(T entity)
    {
        try
        {
            await dbSet.AddAsync(entity);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Add method error", typeof(T).Name);
            return false;
        }
    }

    public virtual Task<bool> Update(T entity)
    {
        throw new NotImplementedException();
    }
    public virtual Task<bool> Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}
