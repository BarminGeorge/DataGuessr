using Application.Extensions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public static class ImageEndpoints
{
    public static IEndpointRouteBuilder MapImageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api");

        group.MapGet("avatars/{id:guid}", GetAvatarFile);
        group.MapGet("questions/{id:guid}", GetQuestionImage);

        return app;
    }

    private static async Task<IResult> GetAvatarFile([FromRoute] Guid id, [FromServices] IImageService imageService)
    {
        return (await imageService.GetAvatar(id))
            .ToResult();
    }

    private static async Task<IResult> GetQuestionImage([FromRoute] Guid id, [FromServices] IImageService imageService)
    {
        return (await imageService.GetQuestionImage(id))
            .ToResult();
    }
}