using Domain.Common;
using Domain.Enums;

namespace Application.Extensions;

public static class ResultExtensions
{
    private static IResult CreateErrorResult(string errorMessage, ErrorType errorType)
    {
        var errorResponse = new { error = errorMessage };
        
        return errorType switch
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

    public static IResult ToResult<T>(this OperationResult<T> result)
    {
        return result.Success 
            ? Results.Ok(result.ResultObj) 
            : CreateErrorResult(result.ErrorMessage, result.ErrorType);
    }

    public static IResult ToResult(this OperationResult result)
    {
        return result.Success 
            ? Results.Ok() 
            : CreateErrorResult(result.ErrorMessage, result.ErrorType);
    }

    public static IResult ToResult(this OperationResult<FileStream> result)
    {
        return result is { Success: true, ResultObj: not null }
            ? Results.File(result.ResultObj)
            : CreateErrorResult(result.ErrorMessage, result.ErrorType);
    }
}