using Application.Extensions;
using Domain.Common;

namespace Application.Filters;

public class ResultConversionFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is OperationResult operationResult)
            return operationResult.ToResult();

        if (result != null 
            && result.GetType().IsGenericType 
            && result.GetType().GetGenericTypeDefinition() == typeof(OperationResult<>))
        {
            var method = typeof(ResultExtensions)
                .GetMethod("ToResult", 1, [result.GetType()]);
            
            if (method != null)
            {
                var genericMethod = method.MakeGenericMethod(result.GetType().GetGenericArguments()[0]);
                return genericMethod.Invoke(null, [result]);
            }
        }

        return result;
    }
}