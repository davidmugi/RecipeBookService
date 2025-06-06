using System.Reflection;

namespace RecipeBookService.Services;

public class SqlQueryProvider : ISqlQueryProvider
{
    private readonly Assembly _assembly;

    public SqlQueryProvider(Assembly assembly = null)
    {
        _assembly = assembly ?? Assembly.GetExecutingAssembly();
    }

    public async Task<string> GetQueryAsync(string resourceName)
    {
        var fullResourceName = _assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

        if (fullResourceName == null)
            throw new FileNotFoundException($"Embedded SQL resource '{resourceName}' not found.");

        await using var stream = _assembly.GetManifestResourceStream(fullResourceName);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}