namespace Domain.Common
{
    public static class OperationResultRetryExtensions
    {
        public static async Task<OperationResult<TData>> WithRetry<TData>(
            this Func<Task<OperationResult<TData>>> operation,
            int maxRetries = 3,
            TimeSpan delay = default)
        {
            var result = OperationResult<TData>.Error("No results found");

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    result = await operation();
                    if (result.Success)
                        return result;
                }
                catch (Exception ex)
                {
                    result = OperationResult<TData>.Error(ex.ToString());
                    if (attempt < maxRetries)
                        await Task.Delay(delay);
                }
            }

            return result;
        }

        public static async Task<OperationResult> WithRetry(
            this Func<Task<OperationResult>> operation,
            int maxRetries = 3,
            TimeSpan delay = default)
        {
            var result = OperationResult.Error("No results found");

            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    result = await operation();
                    if (result.Success)
                        return result;
                }
                catch (Exception ex)
                {
                    result = OperationResult.Error(ex.ToString());
                    if (attempt < maxRetries)
                        await Task.Delay(delay);
                }
            }

            return result;
        }
    }
}