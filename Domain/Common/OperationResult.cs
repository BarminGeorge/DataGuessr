namespace Domain.Common;

public record OperationResult<T>(bool Success, T? ResultObj = default, string ErrorMsg = "")
{
    public static OperationResult<T> Ok(T resultObj) => new(true, resultObj);
    public static OperationResult<T> Error(string errorMsg) => new(false, default, errorMsg);

    public static async Task<OperationResult<T>> TryAsync(Func<Task<T>> operation)
    {
        try
        {
            var result = await operation();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Error(ex.ToString());
        }
    }
}

public record OperationResult(bool Success, string ErrorMsg = "")
{
    public static OperationResult Ok() => new(true);
    public static OperationResult Error(string errorMsg) => new(false, errorMsg);

    public static async Task<OperationResult> TryAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return Ok();
        }
        catch (Exception ex)
        {
            return Error(ex.ToString());
        }
    }
}