using RecipeBookService.DTOs;
using RecipeBookService.Entities;

namespace RecipeBookService.Profiles;

public class RecipeProfile : AutoMapper.Profile
{
    public RecipeProfile()
    {
        CreateMap<Recipe, RecipeDTO>()
            .ReverseMap();

        CreateMap<CreateRecipeDTO, Recipe>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src =>
                src.Ingredients.Select(ingredient => new Ingredient
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Price = ingredient.Price
                })));
    }
}