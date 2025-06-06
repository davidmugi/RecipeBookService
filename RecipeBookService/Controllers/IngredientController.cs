using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Services;

namespace RecipeBookService.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/ingredient")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateIngredient([FromBody] IngredientDTO ingredientDto)
    {
        var response = await _ingredientService.CreateIngredientAsync(ingredientDto);

        if (response.StatusCode == (int)InternalStatusCode.Success) return Ok(response);

        return BadRequest(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateIngredient([FromBody] IngredientDTO ingredientDto)
    {
        var response = await _ingredientService.UpdateIngredientAsync(ingredientDto);

        if (response.StatusCode == (int)InternalStatusCode.Success) return Ok(response);

        return BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredientById(Guid id)
    {
        var response = await _ingredientService.GetIngredientById(id);

        return Ok(response);
    }
}