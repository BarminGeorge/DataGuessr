using Application.Extensions;
using Application.Requests_Responses;
using Application.Services;
using Domain.Common;
using Domain.Entities;
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

    private static async Task<OperationResult> Register([FromForm] RegisterUserRequest request, UserService userService, CancellationToken ct)
    {
        return await userService.Register(request.Login, request.Password, request.PlayerName, request.Avatar, ct);
    }

    private static async Task<OperationResult> Login(LoginUserRequest request, UserService userService, HttpContext context, CancellationToken ct)
    {
        var token = await userService.Login(request.Login, request.Password, ct);
        if (!token.Success || token.ResultObj is null)
            return OperationResult.Error(token.ErrorMsg);
        context.Response.Cookies.Append("", token.ResultObj);
        return OperationResult.Ok();
    }

    private static async Task<OperationResult> UpdateUser([FromForm] UpdateUserRequest request, UserService userService, CancellationToken ct)
    {
        return await userService.UpdateUser(request.UserId, request.PlayerName, request.Avatar, ct);
    }

    private static async Task<OperationResult<User>> CreateGuest([FromForm] CreateGuestRequest request, UserService userService, CancellationToken ct)
    {
        return await userService.CreateGuest(request.PlayerName, request.Avatar, ct);
    }
}