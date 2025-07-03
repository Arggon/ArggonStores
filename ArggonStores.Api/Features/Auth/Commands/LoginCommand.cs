using MediatR;
using Microsoft.AspNetCore.Identity;
using ArggonStores.Api.Models;
using ArggonStores.Api.Services;

namespace ArggonStores.Api.Features.Auth.Commands;

public class LoginCommand : IRequest<LoginCommandResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public AuthResponse? AuthResponse { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new LoginCommandResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return new LoginCommandResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            var token = _tokenService.GenerateToken(user);
            
            return new LoginCommandResult
            {
                Success = true,
                AuthResponse = new AuthResponse
                {
                    Token = token,
                    Email = user.Email!,
                    FirstName = user.FirstName!,
                    LastName = user.LastName!,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return new LoginCommandResult
            {
                Success = false,
                ErrorMessage = "Internal server error"
            };
        }
    }
}