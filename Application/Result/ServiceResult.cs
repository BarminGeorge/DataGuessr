namespace Application.Result;

public record ServiceResult<T>(bool Success, T? ResultObj = default, string ErrorMsg = "")
{   
    public static ServiceResult<T> Ok(T resultObj) => new (true, resultObj);
    public static ServiceResult<T> Error(string errorMsg) => new (false, default, errorMsg);
}

public record ServiceResult(bool Success, string ErrorMsg = "")
{
    public static ServiceResult Ok() => new(true);
    public static ServiceResult Error(string errorMsg) => new(false, errorMsg);
}