using MediatR;
using Microsoft.AspNetCore.Identity;
using ArggonStores.Api.Models;

namespace ArggonStores.Api.Features.Auth.Queries;

public class GetCurrentUserQuery : IRequest<GetCurrentUserQueryResult>
{
    public string UserId { get; set; } = string.Empty;
}

public class GetCurrentUserQueryResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public CurrentUserDto? User { get; set; }
}

public class CurrentUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserQueryResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<GetCurrentUserQueryResult> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return new GetCurrentUserQueryResult
                {
                    Success = false,
                    ErrorMessage = "User ID is required"
                };
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new GetCurrentUserQueryResult
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            return new GetCurrentUserQueryResult
            {
                Success = true,
                User = new CurrentUserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName!,
                    LastName = user.LastName!,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return new GetCurrentUserQueryResult
            {
                Success = false,
                ErrorMessage = "Internal server error"
            };
        }
    }
}