using Microsoft.AspNetCore.Mvc;
using RecipeBookService.DTOs;
using RecipeBookService.Services;

namespace RecipeBookService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDto)
    {
        var response = await _authenticationService.LoginRequest(loginRequestDto);

        return Ok(response);
    }
}