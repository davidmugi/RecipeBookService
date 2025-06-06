using RecipeBookService.DTOs;

namespace RecipeBookService.Services;

public interface IRecipeService
{
    Task<BaseDTO<RecipeDTO>> CreateRecipeAsync(CreateRecipeDTO recipeDto);

    Task<BaseDTO<RecipeDTO>> UpdateRecipeAsync(CreateRecipeDTO recipeDto);

    Task<BaseDTO<RecipeDTO>> GetRecipeByIdAsync(Guid id);

    Task<BaseDTO<List<TopRecipeDTO>>> GetTopThreeExpensiveRecipesAsync();

    Task<BaseDTO<PaginatedList<RecipeDTO>>> GetRecipeWithIngredientsPriceAbove10Async(int page, int pageSize);

    Task<BaseDTO<MostUsedIngredientDTO>> GetMostMostUsedIngredientAsync();
}