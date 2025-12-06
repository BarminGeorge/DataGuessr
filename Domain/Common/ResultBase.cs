using Domain.Enums;

namespace Domain.Common;

public abstract record ResultBase<TResult>(bool Success, string ErrorMessage = "", ErrorType ErrorType = ErrorType.None)
    where TResult : ResultBase<TResult>
{
    public TResult Validation(string message = "Ошибка валидации")
        => CreateError(message, ErrorType.Validation);
    
    public TResult Unauthorized(string message = "Неправильный пароль")
        => CreateError(message, ErrorType.Unauthorized);
    
    public TResult NotFound(string message = "Ресурс не найден")
        => CreateError(message, ErrorType.NotFound);
        
    public TResult Forbidden(string message = "Доступ запрещен")
        => CreateError(message, ErrorType.Forbidden);
        
    public TResult AlreadyExists(string message = "Ресурс уже существует")
        => CreateError(message, ErrorType.AlreadyExists);
        
    public TResult InvalidOperation(string message = "Недопустимая операция")
        => CreateError(message, ErrorType.InvalidOperation);
      
    public TResult InternalError(string message = "Внутренняя ошибка")
        => CreateError(message, ErrorType.InternalError);
        
    public TResult ExternalServiceError(string message = "Ошибка внешнего сервиса")
        => CreateError(message, ErrorType.ExternalServiceError);
        
    public TResult ServiceUnavailable(string message = "Сервис недоступен")
        => CreateError(message, ErrorType.ServiceUnavailable);

    protected abstract TResult CreateError(string message, ErrorType errorType = ErrorType.InternalError);
    
    public OperationResult<TNew> ConvertToOperationResult<TNew>() 
    {
        return Success
            ? throw new InvalidCastException("Cannot convert success result")
            : new OperationResult<TNew>(false, default, ErrorMessage, ErrorType);
    }
}