using MediatR;
using Microsoft.AspNetCore.Identity;
using ArggonStores.Api.Models;
using ArggonStores.Api.Services;

namespace ArggonStores.Api.Features.Auth.Commands;

public class RegisterCommand : IRequest<RegisterCommandResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class RegisterCommandResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public AuthResponse? AuthResponse { get; set; }
    public IEnumerable<IdentityError>? Errors { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService tokenService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<RegisterCommandResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new RegisterCommandResult
                {
                    Success = false,
                    ErrorMessage = "User with this email already exists"
                };
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true // For demo purposes
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new RegisterCommandResult
                {
                    Success = false,
                    ErrorMessage = "Failed to create user",
                    Errors = result.Errors
                };
            }

            var token = _tokenService.GenerateToken(user);
            
            return new RegisterCommandResult
            {
                Success = true,
                AuthResponse = new AuthResponse
                {
                    Token = token,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return new RegisterCommandResult
            {
                Success = false,
                ErrorMessage = "Internal server error"
            };
        }
    }
}