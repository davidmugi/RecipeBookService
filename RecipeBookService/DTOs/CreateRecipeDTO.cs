namespace RecipeBookService.DTOs;

public class CreateRecipeDTO
{
    public Guid? Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int ServingSize { get; set; }

    public int PrepTimeMin { get; set; }

    public int CookTimeMin { get; set; }

    public List<CreateRecipeIngredientsDTO> Ingredients { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }
}