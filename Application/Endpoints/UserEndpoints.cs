using System.ComponentModel.DataAnnotations;
using Application.Services;

namespace Application.EndPoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Register);
        app.MapPost("login", Login);
        
        return app;
    }

    private static async Task<IResult> Register(RegisterUserRequest request, UserService userService)
    {
        await userService.Register(request.Username, request.Password);
        return Results.Ok();
    }

    private static async Task<IResult> Login(LoginUserRequest request, UserService userService, HttpContext context)
    {
        var token = await userService.Login(request.Username, request.Password);
        context.Response.Cookies.Append("", token);
        return Results.Ok(token);
    }
}

public record RegisterUserRequest([Required] string Username, [Required] string Password);
public record LoginUserRequest([Required] string Username, [Required] string Password);