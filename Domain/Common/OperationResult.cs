using Domain.Enums;

namespace Domain.Common;

public record OperationResult<T>(bool Success, T? ResultObj = default, string ErrorMessage = "", ErrorType ErrorType = ErrorType.None) 
    : ResultBase<OperationResult<T>>(Success, ErrorMessage, ErrorType)
{
    public static OperationResult<T> Ok(T resultObj) 
        => new(true, resultObj);

    public static OperationResult<T> Error => new(false);
    
    protected override OperationResult<T> CreateError(string errorMsg, ErrorType errorType = ErrorType.InternalError) 
        => new(false, default, errorMsg, errorType);
    
    public static async Task<OperationResult<T>> TryAsync(Func<Task<T>> operation)
    {
        try
        {
            var result = await operation();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Error.ExternalServiceError(ex.Message);
        }
    }

    public static implicit operator OperationResult(OperationResult<T> result)
    {
        return result.Success 
            ? throw new InvalidCastException("Cannot convert success result") 
            : new OperationResult(false, result.ErrorMessage, result.ErrorType);
    }
}

public record OperationResult(bool Success, string ErrorMessage = "", ErrorType ErrorType = ErrorType.None) 
    : ResultBase<OperationResult>(Success, ErrorMessage, ErrorType)
{
    public static OperationResult Ok() => new(true);
    public static OperationResult Error => new(false);
    
    protected override OperationResult CreateError(string errorMsg, ErrorType errorType = ErrorType.InternalError) 
        => new(false, errorMsg, errorType);
    
    public static async Task<OperationResult> TryAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return Ok();
        }
        catch (Exception ex)
        {
            return Error.InternalError(ex.Message);
        }
    }
}