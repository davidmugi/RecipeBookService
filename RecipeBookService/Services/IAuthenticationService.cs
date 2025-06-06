using RecipeBookService.DTOs;

namespace RecipeBookService.Services;

public interface IAuthenticationService
{
    Task<BaseDTO<LoginResponseDTO>> LoginRequest(LoginRequestDTO request);

    string GetUserEmail();
}