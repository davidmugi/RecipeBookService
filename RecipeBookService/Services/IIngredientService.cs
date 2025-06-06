using RecipeBookService.DTOs;
using RecipeBookService.Entities;

namespace RecipeBookService.Services;

public interface IIngredientService
{
    Task<BaseDTO<IngredientDTO>> CreateIngredientAsync(IngredientDTO ingredientDto);

    Task<BaseDTO<IngredientDTO>> UpdateIngredientAsync(IngredientDTO ingredientDto);

    Task<BaseDTO<IngredientDTO>> GetIngredientById(Guid id);

    Task<List<Ingredient>> GetOrAddIngredients(List<CreateRecipeIngredientsDTO> ingredientDtoList, string createdBy);
}