namespace Application.Requests_Responses;

public record DataResponse<T>(T Data, bool Success, string ErrorMessage = "") 
{
    public static DataResponse<T> CreateSuccess(T data) => new(data, true);
    public static DataResponse<T> CreateFailure(string errorMessage) => new(default, false, errorMessage);
}

public record EmptyResponse(bool Success, string ErrorMessage = "") 
{
    public static EmptyResponse CreateSuccess() => new(true);
    public static EmptyResponse CreateFailure(string errorMessage) => new(false, errorMessage);
}