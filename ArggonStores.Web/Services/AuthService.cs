using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using ArggonStores.Web.Models;

namespace ArggonStores.Web.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginModel model);
    Task<AuthResult> RegisterAsync(RegisterModel model);
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    event Action<bool> AuthStateChanged;
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public event Action<bool> AuthStateChanged = delegate { };

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage, ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<AuthResult> LoginAsync(LoginModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                email = model.Email,
                password = model.Password
            }, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(content, _jsonOptions);

                if (authResponse != null)
                {
                    await _localStorage.SetItemAsync("authToken", authResponse.Token);
                    SetAuthorizationHeader(authResponse.Token);
                    
                    var user = new UserInfo
                    {
                        Email = authResponse.Email,
                        FirstName = authResponse.FirstName,
                        LastName = authResponse.LastName
                    };

                    AuthStateChanged.Invoke(true);
                    return new AuthResult
                    {
                        IsSuccess = true,
                        Message = "Login successful",
                        Token = authResponse.Token,
                        User = user
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = "Login failed";
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponseDto>(errorContent, _jsonOptions);
                if (errorResponse?.Message != null)
                    errorMessage = errorResponse.Message;
            }
            catch { }

            return new AuthResult
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return new AuthResult
            {
                IsSuccess = false,
                Message = "An error occurred during login"
            };
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
            {
                email = model.Email,
                password = model.Password,
                firstName = model.FirstName,
                lastName = model.LastName
            }, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(content, _jsonOptions);

                if (authResponse != null)
                {
                    await _localStorage.SetItemAsync("authToken", authResponse.Token);
                    SetAuthorizationHeader(authResponse.Token);
                    
                    var user = new UserInfo
                    {
                        Email = authResponse.Email,
                        FirstName = authResponse.FirstName,
                        LastName = authResponse.LastName
                    };

                    AuthStateChanged.Invoke(true);
                    return new AuthResult
                    {
                        IsSuccess = true,
                        Message = "Registration successful",
                        Token = authResponse.Token,
                        User = user
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = "Registration failed";
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponseDto>(errorContent, _jsonOptions);
                if (errorResponse?.Message != null)
                    errorMessage = errorResponse.Message;
            }
            catch { }

            return new AuthResult
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return new AuthResult
            {
                IsSuccess = false,
                Message = "An error occurred during registration"
            };
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        AuthStateChanged.Invoke(false);
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                return null;

            SetAuthorizationHeader(token);
            
            var response = await _httpClient.GetAsync("api/auth/me");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<UserInfoDto>(content, _jsonOptions);
                
                if (userResponse != null)
                {
                    return new UserInfo
                    {
                        Id = userResponse.Id,
                        Email = userResponse.Email,
                        FirstName = userResponse.FirstName,
                        LastName = userResponse.LastName,
                        CreatedAt = userResponse.CreatedAt
                    };
                }
            }
            else
            {
                await LogoutAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            await LogoutAsync();
        }

        return null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(token))
            return false;

        SetAuthorizationHeader(token);
        return true;
    }

    private void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
    }

    private class ErrorResponseDto
    {
        public string Message { get; set; } = string.Empty;
    }

    private class UserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}