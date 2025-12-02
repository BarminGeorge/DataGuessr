using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.PostgreSQL.Repositories;

public class AvatarRepository : IAvatarRepository
{
    public Task<OperationResult<Avatar>> SaveUserAvatarAsync(IFormFile avatar, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> DeleteUserAvatarAsync(string filename, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}