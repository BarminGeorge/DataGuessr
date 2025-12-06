using Domain.Common;
using Domain.Enums;

namespace Application.Extensions;

public static class ResultExtensions
{
    public static IResult ToResult<T>(this OperationResult<T> result)
    {
        if (result.Success)
            return Results.Ok(result.ResultObj);

        var errorResponse = new { error = result.ErrorMessage };
        
        return result.ErrorType switch
        {
            ErrorType.Validation => Results.BadRequest(errorResponse),
            ErrorType.Unauthorized => Results.Json(errorResponse, statusCode: 401),
            ErrorType.Forbidden => Results.Json(errorResponse, statusCode: 403),
            ErrorType.NotFound => Results.NotFound(errorResponse),
            ErrorType.AlreadyExists => Results.Conflict(errorResponse),
            ErrorType.InvalidOperation => Results.BadRequest(errorResponse),
            ErrorType.ExternalServiceError => Results.Json(errorResponse, statusCode: 502),
            ErrorType.ServiceUnavailable => Results.Json(errorResponse, statusCode: 503),
            _ => Results.Json(errorResponse, statusCode: 500)
        };
    }

    public static IResult ToResult(this OperationResult result)
    {
        if (result.Success)
            return Results.Ok();

        var errorResponse = new { error = result.ErrorMessage };
        
        return result.ErrorType switch
        {
            ErrorType.Validation => Results.BadRequest(errorResponse),
            ErrorType.Unauthorized => Results.Json(errorResponse, statusCode: 401),
            ErrorType.Forbidden => Results.Json(errorResponse, statusCode: 403),
            ErrorType.NotFound => Results.NotFound(errorResponse),
            ErrorType.AlreadyExists => Results.Conflict(errorResponse),
            ErrorType.InvalidOperation => Results.BadRequest(errorResponse),
            ErrorType.ExternalServiceError => Results.Json(errorResponse, statusCode: 502),
            ErrorType.ServiceUnavailable => Results.Json(errorResponse, statusCode: 503),
            _ => Results.Json(errorResponse, statusCode: 500)
        };
    }
}