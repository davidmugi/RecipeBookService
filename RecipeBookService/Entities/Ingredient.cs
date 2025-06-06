namespace RecipeBookService.Entities;

public class Ingredient
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public List<Recipe> Recipes { get; set; }

    public string CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}