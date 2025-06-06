using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Entities;
using RecipeBookService.Repositories;

namespace RecipeBookService.Services;

public class IngredientService : IIngredientService
{
    private readonly IAuthenticationService _authenticationService;

    private readonly IMapper _mapper;

    private readonly RecipeBookDbContext _recipeBookDbContext;
    private readonly IRepository<Ingredient> _repository;

    public IngredientService(IRepository<Ingredient> repository, IMapper mapper,
        RecipeBookDbContext recipeBookDbContext,
        IAuthenticationService authenticationService)
    {
        _repository = repository;
        _mapper = mapper;
        _recipeBookDbContext = recipeBookDbContext;
        _authenticationService = authenticationService;
    }

    public async Task<BaseDTO<IngredientDTO>> CreateIngredientAsync(IngredientDTO ingredientDto)
    {
        var ingredient = _mapper.Map<Ingredient>(ingredientDto);
        ingredient.CreatedDate = DateTime.Now;
        ingredient.ModifiedDate = DateTime.Now;

        var email = _authenticationService.GetUserEmail();
        ingredient.CreatedBy = email;
        ingredient.ModifiedBy = email;

        await _repository.AddAsync(ingredient);

        return await _recipeBookDbContext.SaveChangesAsync() > 0
            ? new BaseDTO<IngredientDTO>((int)InternalStatusCode.Success, "Ingredient created successful",
                _mapper.Map<IngredientDTO>(ingredient))
            : new BaseDTO<IngredientDTO>((int)InternalStatusCode.BadRequest, "Ingredient creation failed", null);
    }

    public async Task<BaseDTO<IngredientDTO>> UpdateIngredientAsync(IngredientDTO ingredientDto)
    {
        var persistedIngredient = await _repository.GetByIdAsync(ingredientDto.Id);

        var email = _authenticationService.GetUserEmail();
        persistedIngredient.ModifiedBy = email;
        persistedIngredient.ModifiedDate = DateTime.Now;
        persistedIngredient.Name = ingredientDto.Name;
        persistedIngredient.Price = ingredientDto.Price;

        await _repository.UpdateAsync(persistedIngredient);

        return await _recipeBookDbContext.SaveChangesAsync() > 0
            ? new BaseDTO<IngredientDTO>((int)InternalStatusCode.Success, "Ingredient updated successful",
                _mapper.Map<IngredientDTO>(persistedIngredient))
            : new BaseDTO<IngredientDTO>((int)InternalStatusCode.BadRequest, "Ingredient update failed", null);
    }

    public async Task<BaseDTO<IngredientDTO>> GetIngredientById(Guid id)
    {
        var ingredient = await _repository.GetByIdAsync(id);

        return new BaseDTO<IngredientDTO>((int)InternalStatusCode.Success, "Ingredient fetched successful",
            _mapper.Map<IngredientDTO>(ingredient));
    }

    /*
     * The assumption here is that when creating the recipe and the ingredient does not exist
     * we need to add it instead of failing the request, hence why we query the ids first then and
     * the ones that do not exist.
     */
    public async Task<List<Ingredient>> GetOrAddIngredients(List<CreateRecipeIngredientsDTO> ingredientDtoList,
        string createdBy)
    {
        var existingIngredients = await _recipeBookDbContext.Ingredients
            .Where(i => ingredientDtoList.Select(x => x.Id).Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        var newRecipeIngredients = new List<Ingredient>();

        foreach (var ingredientDto in ingredientDtoList)
        {
            Ingredient ingredient = null;

            if (existingIngredients.TryGetValue(ingredientDto.Id, out var existingIngredient))
            {
                newRecipeIngredients.Add(existingIngredient);
            }
            else
            {
                ingredient = new Ingredient
                {
                    Name = ingredientDto.Name,
                    Price = ingredientDto.Price,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                await _repository.AddAsync(ingredient);

                newRecipeIngredients.Add(ingredient);
            }
        }

        return newRecipeIngredients;
    }
}