namespace Application.Result;

public static class OperationResultRetryExtensions
{
    public static async Task<OperationResult<T>> WithRetry<T>(this Task<OperationResult<T>> task, 
        int maxRetries = 3,
        TimeSpan delay = default,
        Func<OperationResult<T>, bool>? shouldRetry = null)
    {
        var result = await task;
        
        for (var attempt = 1; attempt <= maxRetries && ShouldRetry(result, shouldRetry); attempt++)
        {
            await Task.Delay(delay * attempt);
            result = await task;
        }
        
        return result;
    }
    
    public static async Task<OperationResult> WithRetry(this Task<OperationResult> task,
        int maxRetries = 3,
        TimeSpan delay = default,
        Func<OperationResult, bool>? shouldRetry = null)
    {
        var result = await task;
        
        for (var attempt = 1; attempt <= maxRetries && ShouldRetry(result, shouldRetry); attempt++)
        {
            await Task.Delay(delay * attempt);
            result = await task;
        }
        
        return result;
    }
    
    private static bool ShouldRetry<T>(OperationResult<T> result, Func<OperationResult<T>, bool>? shouldRetry) =>
        !result.Success && (shouldRetry?.Invoke(result) ?? true);
    
    private static bool ShouldRetry(OperationResult result, Func<OperationResult, bool>? shouldRetry) =>
        !result.Success && (shouldRetry?.Invoke(result) ?? true);
}