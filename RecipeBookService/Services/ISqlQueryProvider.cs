namespace RecipeBookService.Services;

public interface ISqlQueryProvider
{
    Task<string> GetQueryAsync(string resourceName);
}