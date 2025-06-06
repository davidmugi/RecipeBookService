using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Entities;
using RecipeBookService.Repositories;
using RecipeBookService.Services;

namespace RecipeBookServiceUnitTests.Services;

public class IngredientServiceTests
{
    private readonly Mock<IRepository<Ingredient>> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly RecipeBookDbContext _dbContext;
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        var options = new DbContextOptionsBuilder<RecipeBookDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RecipeBookDbContext(options);

        _service = new IngredientService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _dbContext,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateIngredientAsync_ShouldReturnSuccess_WhenSaveSucceeds()
    {
        var dto = new IngredientDTO { Name = "Oil", Price = 8 };
        var mapped = CreateIngredient();

        _mapperMock.Setup(m => m.Map<Ingredient>(dto)).Returns(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Ingredient>()))
            .Callback<Ingredient>(ingredient => _dbContext.Ingredients.Add(ingredient))
            .Returns(Task.CompletedTask);


        var result = await _service.CreateIngredientAsync(dto);

        Assert.Equal((int)InternalStatusCode.Success, result.StatusCode);
        Assert.Contains("successful", result.Message);
    }

    [Fact]
    public async Task CreateIngredientAsync_ShouldReturnBadRequest_WhenSaveFails()
    {
        var dto = new IngredientDTO { Name = "Salt", Price = 2 };
        var mapped = CreateIngredient();

        _mapperMock.Setup(m => m.Map<Ingredient>(dto)).Returns(mapped);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        _repositoryMock.Setup(r => r.AddAsync(mapped)).Returns(Task.CompletedTask);
        
        var result = await _service.CreateIngredientAsync(dto);

        Assert.Equal((int)InternalStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task UpdateIngredientAsync_ShouldReturnSuccess_WhenSaveSucceeds()
    {
        var id = Guid.NewGuid();
        var existing = CreateIngredient();
        await _dbContext.Ingredients.AddAsync(existing);
        await _dbContext.SaveChangesAsync();

        var dto = new IngredientDTO { Id = id, Name = "New", Price = 5 };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");

        var result = await _service.UpdateIngredientAsync(dto);

        Assert.Equal((int)InternalStatusCode.Success, result.StatusCode);
        Assert.Equal("New", existing.Name);
        Assert.Equal(5, existing.Price);
    }

    [Fact]
    public async Task UpdateIngredientAsync_ShouldReturnBadRequest_WhenSaveFails()
    {
        var ingredient = CreateIngredient();
        
        var dto = new IngredientDTO { Id = ingredient.Id, Name = "Rice", Price = 6,CreatedBy = "user@example.com"};

        _repositoryMock.Setup(r => r.GetByIdAsync(ingredient.Id)).ReturnsAsync(ingredient);
        _authServiceMock.Setup(a => a.GetUserEmail()).Returns("user@example.com");
        
        var result = await _service.UpdateIngredientAsync(dto);

        Assert.Equal((int)InternalStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetIngredientById_ShouldReturnMappedDto()
    {
        var ingredient = CreateIngredient();
        var dto = new IngredientDTO { Id = ingredient.Id, Name = "Salt", Price = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(ingredient.Id)).ReturnsAsync(ingredient);
        _mapperMock.Setup(m => m.Map<IngredientDTO>(ingredient)).Returns(dto);

        var result = await _service.GetIngredientById(ingredient.Id);

        Assert.Equal(dto.Id, result.Data.Id);
        Assert.Equal(dto.Name, result.Data.Name);
    }

    [Fact]
    public async Task GetOrAddIngredients_ShouldReturnAllIngredients_AndCreateIfMissing()
    {
        var existingId = Guid.NewGuid();
        var newId = Guid.NewGuid();

        _dbContext.Ingredients.Add(new Ingredient { Id = existingId, Name = "Flour", Price = 5, CreatedBy = "user@example.com"});
        await _dbContext.SaveChangesAsync();

        var input = new List<CreateRecipeIngredientsDTO>
        {
            new() { Id = existingId, Name = "Flour", Price = 5 },
            new() { Id = newId, Name = "Sugar", Price = 10 }
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Ingredient>())).Returns(Task.CompletedTask);

        var result = await _service.GetOrAddIngredients(input, "user@example.com");

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Id == existingId);
        Assert.Contains(result, r => r.Name == "Sugar");
    }

    private Ingredient CreateIngredient()
    {
        return new Ingredient { Id = Guid.NewGuid(), Name = "Old", Price = 3, CreatedBy = "user@example.com", CreatedDate = DateTime.Now};
    }
}
