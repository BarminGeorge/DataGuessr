namespace Domain.Common
{
    public record OperationResult<T>(bool Success, T? ResultObj = default, string ErrorMsg = "")
    {
        public static OperationResult<T> Ok(T resultObj) => new(true, resultObj);
        public static OperationResult<T> Error(string errorMsg) => new(false, default, errorMsg);
    }

    public record OperationResult(bool Success, string ErrorMsg = "")
    {
        public static OperationResult Ok() => new(true);
        public static OperationResult Error(string errorMsg) => new(false, errorMsg);
    }
}
