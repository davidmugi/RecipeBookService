using System.Net.Http.Json;
using RecipeBookService.Configurations;
using RecipeBookService.DTOs;
using Xunit;

namespace RecipeBookServiceIntegratedTest;

public class AuthControllerTest
{
    [Fact]
    public async Task LoginRequest_ShouldReturnToken()
    {
        var application = new RecipeBookServiceWebApplicationFactory();
        
        var loginRequest = new
        {
            Username = "user@example.com",
            Password = "User@123"
        };

        var client = application.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth", loginRequest);

        response.EnsureSuccessStatusCode();
        
        var loginResponse = await response.Content.ReadFromJsonAsync<BaseDTO<LoginResponseDTO>>();
        
        Assert.Equal((int) InternalStatusCode.Success,loginResponse.StatusCode);
    }
    
    [Fact]
    public async Task LoginRequest_Should401_whenPasswordIsInvalid()
    {
        var application = new RecipeBookServiceWebApplicationFactory();
        
        var loginRequest = new
        {
            Username = "user@example.com",
            Password = "User@3"
        };

        var client = application.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth", loginRequest);
        
        var loginResponse = await response.Content.ReadFromJsonAsync<BaseDTO<LoginResponseDTO>>();
        
        Assert.Equal((int) InternalStatusCode.Unauthorized,loginResponse.StatusCode);
    }
}