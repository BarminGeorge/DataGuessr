using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IImageRepository
{
    public Task<OperationResult<Avatar>> SaveUserAvatarAsync(IFormFile avatarFile, CancellationToken ct);
    Task<OperationResult<FileStream>> GetImageFile(Guid id, ImageType type, CancellationToken ct = default);
    // не сохраняется в бд, файл преносится на диск, возвращает объект Avatar 
    //Task<OperationResult> DeleteUserAvatarAsync(string filename, CancellationToken ct);
}