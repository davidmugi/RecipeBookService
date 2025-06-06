using RecipeBookService.DTOs;
using RecipeBookService.Entities;

namespace RecipeBookService.Profiles;

public class IngredientProfile : AutoMapper.Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDTO>()
            .ReverseMap();
    }
}