namespace RecipeBookService.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid id);

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task<List<T>> RawSql(string query);
}