using MediatR;
using Microsoft.AspNetCore.Identity;
using ArggonStores.Api.Models;

namespace ArggonStores.Api.Features.Auth.Commands;

public class LogoutCommand : IRequest<LogoutCommandResult>
{
}

public class LogoutCommandResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutCommandResult>
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutCommandHandler(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<LogoutCommandResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Since we're using JWT tokens, logout is handled client-side
        // by removing the token from storage
        await _signInManager.SignOutAsync();
        
        return new LogoutCommandResult
        {
            Success = true,
            Message = "Logged out successfully"
        };
    }
}