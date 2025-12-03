using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IAvatarRepository
{
    public Task<OperationResult<Avatar>> SaveUserAvatarAsync(IFormFile avatarFile, CancellationToken ct);
    // не сохраняется в бд, файл преносится на диск, возвращает объект Avatar 
}