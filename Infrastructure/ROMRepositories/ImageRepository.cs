using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.ROMRepositories;

public class ImageRepository : IImageRepository
{
    //TODO: get path from json or .env
    private static readonly string avatarsRoot = Path.Combine(Directory.GetCurrentDirectory(), "files", "avatars");
    private static readonly string questionsImagesRoot = Path.Combine(Directory.GetCurrentDirectory(), "files", "questions");
    
    public ImageRepository()
    {
        Directory.CreateDirectory(avatarsRoot);
        Directory.CreateDirectory(questionsImagesRoot);
    }

    public async Task<OperationResult<Avatar>> SaveUserAvatarAsync(IFormFile avatarFile, CancellationToken ct)
    {
        return await OperationResult<Avatar>.TryAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(avatarFile);
            var avatar = new Avatar(avatarFile.FileName, avatarFile.ContentType);

            var extension = Path.GetExtension(avatarFile.FileName);
            var fileNameOnDisk = $"{avatar.Id}{extension}";
            var fullPath = Path.Combine(avatarsRoot, fileNameOnDisk);
            
            await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await avatarFile.CopyToAsync(stream, ct);
            }

            //await db.Avatars.AddAsync(avatar, ct);
            //await db.SaveChangesAsync(ct);

            return avatar;
        });
    }
    
    public Task<OperationResult> DeleteUserAvatarAsync(string filename, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
    
    public Task<OperationResult<FileStream>> GetImageFile(Guid id, ImageType type, CancellationToken ct = default)
    {
        var directoryPath = GetPath(type);
        if (!Directory.Exists(directoryPath))
            return Task.FromResult(OperationResult<FileStream>.Error.NotFound());
        
        var files = Directory.GetFiles(directoryPath, $"{id}.*");
        
        if (files.Length == 0)
            return Task.FromResult(OperationResult<FileStream>.Error.NotFound());

        var filePath = files[0];
        
        return Task.FromResult(OperationResult<FileStream>.Ok(File.OpenRead(filePath)));
    }

    private static string GetPath(ImageType type)
    {
        return type switch
        {
            ImageType.Avatar => avatarsRoot,
            ImageType.Question => questionsImagesRoot,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Неизвестный тип изображения: {type}")
        };
    }
}