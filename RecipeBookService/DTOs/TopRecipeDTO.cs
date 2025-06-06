namespace RecipeBookService.DTOs;

public class TopRecipeDTO
{
    public Guid RecipeId { get; set; }
    public string RecipeName { get; set; }
    public decimal TotalCost { get; set; }
    public int IngredientCount { get; set; }
}