using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Services;

namespace RecipeBookService.Controllers;

[Authorize]
[ApiController]
[Route("api/recipe")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDTO recipeDto)
    {
        var response = await _recipeService.CreateRecipeAsync(recipeDto);

        if (response.StatusCode == (int)InternalStatusCode.Success) return Ok(response);

        return BadRequest(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRecipe([FromBody] CreateRecipeDTO recipeDto)
    {
        var response = await _recipeService.UpdateRecipeAsync(recipeDto);

        if (response.StatusCode == (int)InternalStatusCode.Success) return Ok(response);

        return BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(Guid id)
    {
        var response = await _recipeService.GetRecipeByIdAsync(id);

        return Ok(response);
    }

    [HttpGet]
    [Route("get-top-three-expensive-recipes")]
    public async Task<IActionResult> GetTopThreeExpensiveRecipe()
    {
        var response = await _recipeService.GetTopThreeExpensiveRecipesAsync();

        return Ok(response);
    }

    [HttpGet]
    [Route("get-recipe-with-ingredients-price-above-ten-dollars/{pageIndex}/{pageSize}")]
    public async Task<IActionResult> GetRecipeWithIngredientsPriceAbove10(int pageIndex, int pageSize)
    {
        var response = await _recipeService.GetRecipeWithIngredientsPriceAbove10Async(pageIndex, pageSize);

        return Ok(response);
    }

    [HttpGet]
    [Route("get-top-most-used-ingredient")]
    public async Task<IActionResult> GetMostMostUsedIngredient()
    {
        var response = await _recipeService.GetMostMostUsedIngredientAsync();

        return Ok(response);
    }
}