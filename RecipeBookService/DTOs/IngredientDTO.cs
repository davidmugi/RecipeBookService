namespace RecipeBookService.DTOs;

public class IngredientDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }
}