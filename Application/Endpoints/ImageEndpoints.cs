using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Application.Endpoints;

public static class ImageEndpoints
{
    private static readonly string avatarsPath = Path.Combine(Directory.GetCurrentDirectory(), "files", "avatars");
    
    public static IEndpointRouteBuilder MapImageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api");
            
        group.MapGet("avatars/{id:guid}", GetAvatarFile);
        group.MapGet("questions/{id:guid}", GetQuestionImage);
        
        return app;
    }

    private static Results<FileStreamHttpResult, NotFound> GetAvatarFile([FromRoute] Guid id)
    {
        if (!Directory.Exists(avatarsPath))
            return TypedResults.NotFound();
        
        var files = Directory.GetFiles(avatarsPath, $"{id}.*");
        
        if (files.Length == 0)
            return TypedResults.NotFound();

        var filePath = files[0];
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        var fileStream = File.OpenRead(filePath);
        return TypedResults.File(fileStream, contentType, enableRangeProcessing: true);
    }

    private static Results<FileStreamHttpResult, NotFound> GetQuestionImage([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }
}