using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace Application.Extensions;

public static class HubValidationExtensions
{
    public static async Task<string?> ValidateRequestAsync<TRequest>(
        this Hub hub, 
        TRequest request, 
        CancellationToken ct = default)
    {
        var serviceProvider = hub.Context.GetHttpContext()?.RequestServices;
        if (serviceProvider == null) 
            return null;

        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator == null) 
            return null;

        var result = await validator.ValidateAsync(request, ct);
        
        return !result.IsValid
            ? string.Join("; ", result.Errors.Select(e => e.ErrorMessage))
            : null;
    }
}