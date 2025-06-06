using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using RecipeBookService.Exceptions;

namespace RecipeBookService.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly JWTService _jwtService;

    private readonly UserManager<IdentityUser> _userManager;

    public AuthenticationService(UserManager<IdentityUser> userManager, JWTService jwtService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseDTO<LoginResponseDTO>> LoginRequest(LoginRequestDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("Invalid login details");

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtService.GenerateToken(user, roles.FirstOrDefault());

        return new BaseDTO<LoginResponseDTO>((int)InternalStatusCode.Success, "Login successful",
            new LoginResponseDTO { Token = token });
    }

    public string GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    }
}