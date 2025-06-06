using Microsoft.EntityFrameworkCore;
using RecipeBookService.Configurations;
using RecipeBookService.Exceptions;

namespace RecipeBookService.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly RecipeBookDbContext _dbContext;

    private readonly ILogger<Repository<T>> _logger;

    public Repository(RecipeBookDbContext dbContext, ILogger<Repository<T>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(id);

        if (entity == null) throw new NotFoundException(typeof(T).Name + " not found with the provided id : " + id);

        return entity;
    }

    public virtual async Task AddAsync(T entity)
    {
        try
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"An exception occurred while updating entity {Name}", typeof(T).Name);
        }
    }

    public virtual Task UpdateAsync(T entity)
    {
        try
        {
            _dbContext.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"An exception occurred while updating entity : {Name}", typeof(T).Name);
        }
        return Task.CompletedTask;
    }

    public virtual async Task<List<T>> RawSql(string query)
    {
        return await _dbContext.Set<T>(query).FromSqlRaw(query).ToListAsync();
    }
}