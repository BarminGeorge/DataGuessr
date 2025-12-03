using Application.Extensions;
using Application.Requests_Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.EndPoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersGroup = app.MapGroup("");
        usersGroup.AddFluentValidationAutoValidation();
        usersGroup.MapPost("register", Register)
            .DisableAntiforgery()
            .WithFormUpload();
        usersGroup.MapPost("login", Login);
        usersGroup.MapPost("{id:guid}/userUpdate", UpdateUser)
            .DisableAntiforgery()
            .WithFormUpload();
        usersGroup.MapPost("guest", CreateGuest)
            .DisableAntiforgery()
            .WithFormUpload();
        
        return app;
    }

    private static async Task<IResult> Register([FromForm] RegisterUserRequest request, UserService userService, CancellationToken ct)
    {
        await userService.Register(request.Login, request.Password, request.PlayerName, request.Avatar, ct);
        return Results.Ok();
    }

    private static async Task<IResult> Login(LoginUserRequest request, UserService userService, HttpContext context, CancellationToken ct)
    {
        var token = await userService.Login(request.Login, request.Password, ct);
        context.Response.Cookies.Append("", token.ResultObj);
        return Results.Ok(token);
    }

    private static async Task<IResult> UpdateUser([FromForm] UpdateUserRequest request, UserService userService, CancellationToken ct)
    {
        var result = await userService.UpdateUser(request.UserId, request.PlayerName, request.Avatar, ct);
        return result.Success 
            ? Results.Ok() 
            : Results.BadRequest(result.ErrorMsg);
    }

    private static async Task<IResult> CreateGuest([FromForm] CreateGuestRequest request, UserService userService, CancellationToken ct)
    {
        var result = await userService.CreateGuest(request.PlayerName, request.Avatar, ct);
        return result.Success
            ? Results.Ok(result.ResultObj)
            : Results.BadRequest(result.ErrorMsg);
    }
}