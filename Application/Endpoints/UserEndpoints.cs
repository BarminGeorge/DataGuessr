using Application.Requests_Responses;
using Application.Services;

namespace Application.EndPoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Register);
        app.MapPost("login", Login);
        app.MapPost("{id:guid}/userUpdate", UpdateUser);
        app.MapPost("guest", CreateGuest);
        
        return app;
    }

    private static async Task<IResult> Register(RegisterUserRequest request, UserService userService, CancellationToken ct)
    {
        await userService.Register(request.PlayerName, request.Password, request.PlayerName, request.Avatar, ct);
        return Results.Ok();
    }

    private static async Task<IResult> Login(LoginUserRequest request, UserService userService, HttpContext context, CancellationToken ct)
    {
        var token = await userService.Login(request.Username, request.Password, ct);
        context.Response.Cookies.Append("", token);
        return Results.Ok(token);
    }

    private static async Task<IResult> UpdateUser(UpdateUserRequest request, UserService userService, CancellationToken ct)
    {
        var result = await userService.UpdateUser(request.UserId, request.Username, request.Avatar, ct);
        return result.Success 
            ? Results.Ok() 
            : Results.BadRequest(result.ErrorMsg);
    }

    private static async Task<IResult> CreateGuest(CreateGuestRequest request, UserService userService, CancellationToken ct)
    {
        var result = await userService.CreateGuest(request.Username, request.Avatar, ct);
        return result.Success
            ? Results.Ok(result.ResultObj)
            : Results.BadRequest(result.ErrorMsg);
    }
}