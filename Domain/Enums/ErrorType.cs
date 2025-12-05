namespace Domain.Enums;

public enum ErrorType
{
    None = 0,
    
    Validation = 400,
    Unauthorized = 401,
    NotFound = 404,
    Forbidden = 403,
    AlreadyExists = 409,
    InvalidOperation = 422,
    
    InternalError = 500,
    ExternalServiceError = 502,
    ServiceUnavailable = 503,
}