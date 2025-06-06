using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Entities;
using RecipeBookService.Repositories;
using RecipeBookService.Services;

namespace RecipeBookServiceUnitTests.Services;

public class RecipeServiceTests
{
    private readonly Mock<IRepository<Recipe>> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly RecipeBookDbContext _dbContext;
    private readonly Mock<IIngredientService> _ingredientServiceMock = new();
    private readonly RecipeService _service;
    private readonly Mock<ISqlQueryProvider> _sqlQueryProvider = new ();

    public RecipeServiceTests()
    {
        var options = new DbContextOptionsBuilder<RecipeBookDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RecipeBookDbContext(options);

        _service = new RecipeService(
            _repositoryMock.Object,
            _dbContext,
            _mapperMock.Object,
            _authServiceMock.Object,
            _ingredientServiceMock.Object,
            _sqlQueryProvider.Object
        );
    }
    
    [Fact]
    public async Task CreateRecipeAsync_ShouldReturnSuccess_WhenIngredientDoNotExistAndSaveSucceeds()
    {
        var dto = CreateRecipeDTO();
        var mapped = CreateRecipe();
        
        var existing = CreateIngredient();
        await _dbContext.Ingredients.AddAsync(existing);
        await _dbContext.SaveChangesAsync();
        
        _mapperMock.Setup(m => m.Map<Recipe>(It.IsAny<CreateRecipeDTO>())).Returns(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _ingredientServiceMock.Setup(i => i.GetOrAddIngredients(dto.Ingredients,"user@example.com")).ReturnsAsync([existing]);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Recipe>()))
            .Callback<Recipe>(recipe => _dbContext.Recipes.Add(recipe))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateRecipeAsync(dto);
        
        Assert.Equal((int) InternalStatusCode.Success, result.StatusCode);
    }
    
    [Fact]
    public async Task CreateRecipeAsync_ShouldReturnSuccess_WhenIngredientExistAndSaveSucceeds()
    {
        var dto = CreateRecipeDTO();
        var mapped = CreateRecipe();
        
        _mapperMock.Setup(m => m.Map<Recipe>(It.IsAny<CreateRecipeDTO>())).Returns(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _ingredientServiceMock.Setup(i => i.GetOrAddIngredients(dto.Ingredients,"user@example.com")).ReturnsAsync(mapped.Ingredients);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Recipe>()))
            .Callback<Recipe>(recipe => _dbContext.Recipes.Add(recipe))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateRecipeAsync(dto);
        
        Assert.Equal((int) InternalStatusCode.Success, result.StatusCode);
    }
    
    [Fact]
    public async Task CreateRecipeAsync_ShouldReturnSuccess_WhenIngredientExistAndSaveFails()
    {
        var dto = CreateRecipeDTO();
        var mapped = CreateRecipe();
        
        _mapperMock.Setup(m => m.Map<Recipe>(It.IsAny<CreateRecipeDTO>())).Returns(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _ingredientServiceMock.Setup(i => i.GetOrAddIngredients(dto.Ingredients,"user@example.com")).ReturnsAsync(mapped.Ingredients);

        var result = await _service.CreateRecipeAsync(dto);
        
        Assert.Equal((int) InternalStatusCode.BadRequest, result.StatusCode);
    }
    
     [Fact]
    public async Task UpdateRecipeAsync_ShouldReturnSuccess_WhenIngredientDoNotExistAndSaveSucceeds()
    {
        var mapped = CreateRecipe();
        await _dbContext.Recipes.AddAsync(mapped);
        await _dbContext.SaveChangesAsync();

        var dto = CreateRecipeDTO();
        dto.Id = mapped.Id;
        
        var existing = CreateIngredient();
        await _dbContext.Ingredients.AddAsync(existing);
        await _dbContext.SaveChangesAsync();
        
        _mapperMock.Setup(m => m.Map<Recipe>(It.IsAny<CreateRecipeDTO>())).Returns(mapped);
        _repositoryMock.Setup(r => r.GetByIdAsync(mapped.Id)).ReturnsAsync(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _ingredientServiceMock.Setup(i => i.GetOrAddIngredients(dto.Ingredients,"user@example.com")).ReturnsAsync([existing]);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
            .Callback<Recipe>(recipe => _dbContext.Recipes.Update(recipe))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateRecipeAsync(dto);
        
        Assert.Equal((int) InternalStatusCode.Success, result.StatusCode);
    }
    
    [Fact]
    public async Task UpdateRecipeAsync_ShouldReturnSuccess_WhenIngredientExistAndSaveSucceeds()
    {
        var mapped = CreateRecipe();
        await _dbContext.Recipes.AddAsync(mapped);
        await _dbContext.SaveChangesAsync();

        var dto = CreateRecipeDTO();
        dto.Id = mapped.Id;
        
        _mapperMock.Setup(m => m.Map<Recipe>(It.IsAny<CreateRecipeDTO>())).Returns(mapped);
        _repositoryMock.Setup(r => r.GetByIdAsync(mapped.Id)).ReturnsAsync(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _ingredientServiceMock.Setup(i => i.GetOrAddIngredients(dto.Ingredients,"user@example.com")).ReturnsAsync(mapped.Ingredients);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Recipe>()))
            .Callback<Recipe>(recipe => _dbContext.Recipes.Update(recipe))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateRecipeAsync(dto);
        
        Assert.Equal((int) InternalStatusCode.Success, result.StatusCode);
    }
    
    [Fact]
    public async Task UpdateRecipeAsync_ShouldReturnSuccess_WhenIngredientExistAndSaveFails()
    {
        var mapped = CreateRecipe();

        var dto = CreateRecipeDTO();
        dto.Id = mapped.Id;
        
        _mapperMock.Setup(m => m.Map<Recipe>(It.IsAny<CreateRecipeDTO>())).Returns(mapped);
        _repositoryMock.Setup(r => r.GetByIdAsync(mapped.Id)).ReturnsAsync(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _ingredientServiceMock.Setup(i => i.GetOrAddIngredients(dto.Ingredients,"user@example.com")).ReturnsAsync(mapped.Ingredients);

        var result = await _service.UpdateRecipeAsync(dto);
        
        Assert.Equal((int) InternalStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_ShouldReturnMappedDto()
    {
        var existing = CreateRecipe();
        var dto = RecipeDTO();
        
        _mapperMock.Setup(m => m.Map<RecipeDTO>(It.IsAny<Recipe>())).Returns(dto);
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        var result = await _service.GetRecipeByIdAsync(existing.Id);
        
        Assert.Equal(dto.Id, result.Data.Id);
        Assert.Equal(dto.Title, result.Data.Title);
    }
    
    private Recipe CreateRecipe()
    {
        var ingredientDTo = new Ingredient{Name = "Old", Price = 3, CreatedBy = "user@example.com"};
        
        return new Recipe()
        {
            Id = Guid.NewGuid(), Title = "Recipe One", Description = "Best Recipe", ServingSize = 2, CookTimeMin = 10, PrepTimeMin = 10,
            CreatedBy = "user@example.com", Ingredients = [ingredientDTo]
        };
    }
    
    private CreateRecipeDTO CreateRecipeDTO()
    {
        var ingredientDTo = new CreateRecipeIngredientsDTO(){Name = "Old", Price = 3 };
        
        return new CreateRecipeDTO()
        {
            Title = "Recipe One", Description = "Best Recipe", ServingSize = 2, CookTimeMin = 10, PrepTimeMin = 10,
            CreatedBy = "user@example.com", Ingredients = [ingredientDTo]
        };
    }
    
    private Ingredient CreateIngredient()
    {
        return new Ingredient { Id = Guid.NewGuid(), Name = "Old", Price = 3, CreatedBy = "user@example.com", CreatedDate = DateTime.Now};
    }
    
    private RecipeDTO RecipeDTO()
    {
        var ingredientDTo = new IngredientDTO{Name = "Old", Price = 3 };
        
        return new RecipeDTO()
        {
            Title = "Recipe One", Description = "Best Recipe", ServingSize = 2, CookTimeMin = 10, PrepTimeMin = 10,
            CreatedBy = "user@example.com", Ingredients = [ingredientDTo]
        };
    }
}