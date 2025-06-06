namespace RecipeBookService.DTOs;

public class CreateRecipeIngredientsDTO
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }
}