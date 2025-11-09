namespace Infrastructure.Interfaces;

public interface IRepository<T> where T : class
{
    Task SaveAsync(T entity, CancellationToken ct = default);
    Task SaveAsync(IEnumerable<T> entities, CancellationToken ct = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> UpdateOrInsertAsync(T entity, CancellationToken ct = default);
    Task<T?> UpdateOrThrowAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task DeleteByIdAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(IEnumerable<T> entities, CancellationToken ct = default);
}