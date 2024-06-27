using DevicesApp.Models;
using System.Linq.Expressions;

namespace DevicesApp.Core.IRepositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includes);
    Task<T?> GetById(Guid id);
    Task<T?> GetById(Guid id, params Expression<Func<T, object>>[] includes);
    Task<T?> Find(Expression<Func<T, bool>> criteria);
    Task<T?> Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> criteria);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
    Task<bool> Add(T entity);
    Task<bool> Upsert(T entity);
    Task<bool> Delete(Guid id);
}
