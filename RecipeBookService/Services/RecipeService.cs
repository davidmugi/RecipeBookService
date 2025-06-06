using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Entities;
using RecipeBookService.Repositories;

namespace RecipeBookService.Services;

public class RecipeService : IRecipeService
{
    private readonly IAuthenticationService _authenticationService;

    private readonly IIngredientService _ingredientService;

    private readonly IMapper _mapper;

    private readonly RecipeBookDbContext _recipeBookDbContext;
    
    private readonly IRepository<Recipe> _repository;

    private readonly ISqlQueryProvider _sqlQueryProvider;

    public RecipeService(IRepository<Recipe> repository, RecipeBookDbContext recipeBookDbContext, IMapper mapper,
        IAuthenticationService authenticationService, IIngredientService ingredientService,
        ISqlQueryProvider sqlQueryProvider)
    {
        _repository = repository;
        _recipeBookDbContext = recipeBookDbContext;
        _mapper = mapper;
        _authenticationService = authenticationService;
        _ingredientService = ingredientService;
        _sqlQueryProvider = sqlQueryProvider;
    }

    public async Task<BaseDTO<RecipeDTO>> CreateRecipeAsync(CreateRecipeDTO recipeDto)
    {
        var recipe = _mapper.Map<Recipe>(recipeDto);
        recipe.CreatedDate = DateTime.Now;
        recipe.ModifiedDate = DateTime.Now;

        var email = _authenticationService.GetUserEmail();
        recipe.CreatedBy = email;

        var ingredientList = await _ingredientService.GetOrAddIngredients(recipeDto.Ingredients, email);
        recipe.Ingredients = ingredientList;

        await _repository.AddAsync(recipe);

        return await _recipeBookDbContext.SaveChangesAsync() > 0
            ? new BaseDTO<RecipeDTO>((int)InternalStatusCode.Success, "Recipe created successful",
                _mapper.Map<RecipeDTO>(recipe))
            : new BaseDTO<RecipeDTO>((int)InternalStatusCode.BadRequest, "Recipe creation failed", null);
    }

    public async Task<BaseDTO<RecipeDTO>> UpdateRecipeAsync(CreateRecipeDTO recipeDto)
    {
        var persistedRecipe = await _repository.GetByIdAsync((Guid)recipeDto.Id);

        var recipe = _mapper.Map<Recipe>(recipeDto);
        persistedRecipe.ModifiedDate = DateTime.Now;
        persistedRecipe.CookTimeMin = recipeDto.CookTimeMin;
        persistedRecipe.ServingSize = recipeDto.ServingSize;
        persistedRecipe.Title = recipeDto.Title;
        persistedRecipe.Description = recipeDto.Description;
        
        var email = _authenticationService.GetUserEmail();
        persistedRecipe.ModifiedBy = email;
        
        await _repository.UpdateAsync(persistedRecipe);

        return await _recipeBookDbContext.SaveChangesAsync() > 0
            ? new BaseDTO<RecipeDTO>((int)InternalStatusCode.Success, "Recipe updated successful",
                _mapper.Map<RecipeDTO>(recipe))
            : new BaseDTO<RecipeDTO>((int)InternalStatusCode.BadRequest, "Recipe update failed", null);
    }

    public async Task<BaseDTO<RecipeDTO>> GetRecipeByIdAsync(Guid id)
    {
        var recipe = await _repository.GetByIdAsync(id);

        return new BaseDTO<RecipeDTO>((int)InternalStatusCode.Success, "Recipe fetched successful",
            _mapper.Map<RecipeDTO>(recipe));
    }

    public async Task<BaseDTO<List<TopRecipeDTO>>> GetTopThreeExpensiveRecipesAsync()
    {
        var query = await _sqlQueryProvider.GetQueryAsync("TopThreeExpensiveRecipes.sql");

        var topRecipeList = await _recipeBookDbContext.Set<TopRecipeDTO>().FromSqlRaw(query).ToListAsync();

        return new BaseDTO<List<TopRecipeDTO>>((int)InternalStatusCode.Success,
            "Top three most expensive recipes fetched successfully",
            topRecipeList);
    }

    public async Task<BaseDTO<PaginatedList<RecipeDTO>>> GetRecipeWithIngredientsPriceAbove10Async(int page,
        int pageSize)
    {
        var recipes = await _recipeBookDbContext.Recipes
            .Where(r => r.Ingredients.Any(i => i.Price > 10))
            .Include(r => r.Ingredients)
            .OrderByDescending(r => r.CreatedDate)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var count = await _recipeBookDbContext.Recipes.CountAsync();

        var paginatedList = new PaginatedList<RecipeDTO>(_mapper.Map<List<RecipeDTO>>(recipes), page, pageSize, count);

        return new BaseDTO<PaginatedList<RecipeDTO>>((int)InternalStatusCode.Success, "Recipes fetched successful",
            paginatedList);
    }

    public async Task<BaseDTO<MostUsedIngredientDTO>> GetMostMostUsedIngredientAsync()
    {
        var query = await _sqlQueryProvider.GetQueryAsync("MostUsedIngredient.sql");

        var mostUsedIngredientList =
            await _recipeBookDbContext.Set<MostUsedIngredientDTO>().FromSqlRaw(query).ToListAsync();

        return new BaseDTO<MostUsedIngredientDTO>((int)InternalStatusCode.Success,
            "Most used Ingredient fetched successfully", mostUsedIngredientList[0]);
    }
}