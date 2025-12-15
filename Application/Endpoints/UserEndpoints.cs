using Application.Extensions;
using Application.Interfaces;
using Application.Mappers;
using Application.Requests;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.EndPoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersGroup = app.MapGroup("api");
            
        usersGroup.AddFluentValidationAutoValidation();
        
        usersGroup.MapPost("register", Register)
            .DisableAntiforgery()
            .WithFormUpload();
        usersGroup.MapPost("login", Login);
        usersGroup.MapPost("userUpdate", UpdateUser)
            .DisableAntiforgery()
            .WithFormUpload();
        usersGroup.MapPost("guest", CreateGuest)
            .DisableAntiforgery()
            .WithFormUpload();
        
        return app;
    }

    private static async Task<IResult> Register([FromForm] RegisterUserRequest request, 
        [FromServices] IUserService userService, CancellationToken ct)
    {
        var result = await userService.Register(request.Login, request.Password, request.PlayerName, request.Avatar, ct);
        
        return result is { Success: true, ResultObj: not null }
            ? Results.Ok(result.ResultObj.ToDto()) 
            : result.ToResult();
    }

    private static async Task<IResult> Login(LoginUserRequest request, 
        [FromServices] IUserService userService, HttpContext context, CancellationToken ct)
    {
        var loginResult = await userService.Login(request.Login, request.Password, ct);
        if (!loginResult.Success)
            return loginResult.ToResult();
        context.Response.Cookies.Append("token", loginResult.ResultObj.token);
        return Results.Ok(loginResult.ResultObj.user.ToDto());
    }

    private static async Task<IResult> UpdateUser([FromForm] UpdateUserRequest request, 
        [FromServices] IUserService userService, CancellationToken ct)
    {
        return (await userService.UpdateUser(request.UserId, request.PlayerName, request.Avatar, ct))
            .ToResult();
    }

    private static async Task<IResult> CreateGuest([FromForm] CreateGuestRequest request, 
        [FromServices] IUserService userService, CancellationToken ct)
    {
        var result = await userService.CreateGuest(request.PlayerName, request.Avatar, ct);
        
        return result is { Success: true, ResultObj: not null }
            ? Results.Ok(result.ResultObj.ToDto()) 
            : result.ToResult();
    }
}