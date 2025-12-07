using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.PostgreSQL.Repositories;

public class AvatarRepository : IAvatarRepository
{
    private readonly AppDbContext db;

    //TODO: get path from json or .env
    private readonly string avatarsRoot = Path.Combine(Directory.GetCurrentDirectory(), "files", "avatars");

    public AvatarRepository(AppDbContext db)
    {
        Directory.CreateDirectory(avatarsRoot);
        this.db = db ?? throw new ArgumentNullException(nameof(db));
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

            // await db.Avatars.AddAsync(avatar, ct);
            // await db.SaveChangesAsync(ct);

            return avatar;
        });
    }
    
    public Task<OperationResult> DeleteUserAvatarAsync(string filename, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}