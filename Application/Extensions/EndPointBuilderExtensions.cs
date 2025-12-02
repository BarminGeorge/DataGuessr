using Microsoft.OpenApi.Models;

namespace Application.Extensions;

public static class EndpointBuilderExtensions
{
    public static RouteHandlerBuilder WithFormUpload(this RouteHandlerBuilder builder)
    {
        return builder.WithOpenApi(operation =>
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new()
                        {
                            Schema = new OpenApiSchema { Type = "object" }
                        }
                    }
                };
                return operation;
            })
            .Accepts<IFormFileCollection>("multipart/form-data")
            .Produces(200)
            .ProducesProblem(400);
    }
}