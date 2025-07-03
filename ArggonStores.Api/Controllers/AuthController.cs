using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ArggonStores.Api.Models;
using ArggonStores.Api.Features.Auth.Commands;
using ArggonStores.Api.Features.Auth.Queries;

namespace ArggonStores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            if (result.Errors != null)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.AuthResponse);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(result.AuthResponse);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var query = new GetCurrentUserQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            if (result.ErrorMessage == "User not found")
            {
                return NotFound();
            }
            return StatusCode(500, new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            id = result.User!.Id,
            email = result.User.Email,
            firstName = result.User.FirstName,
            lastName = result.User.LastName,
            createdAt = result.User.CreatedAt
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutCommand();
        var result = await _mediator.Send(command);

        return Ok(new { message = result.Message });
    }
}