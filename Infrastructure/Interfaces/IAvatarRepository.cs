using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Infrastructure;

public interface IAvatarRepository
{
    public Task<OperationResult<Avatar>> SaveUserAvatarAsync(IFormFile avatar, CancellationToken ct);
    // не сохраняется в бд, файл преносится на диск, возвращает объект Avatar 
}