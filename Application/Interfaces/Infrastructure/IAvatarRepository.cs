using Application.Result;
using Domain.Entities;

namespace Application.Interfaces.Infrastructure;

public interface IAvatarRepository
{
    public Task<OperationResult<Avatar>> SaveUserAvatarAsync(IFormFile avatar, CancellationToken ct);
}