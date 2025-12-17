using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<OperationResult<User>> Register(string login, string password, string playerName, IFormFile image, CancellationToken ct);
    Task<OperationResult<(string token, User user)>> Login(string login, string password, CancellationToken ct);
    Task<OperationResult<string>> UpdateUser(Guid userId, string playerName, IFormFile avatar, CancellationToken ct);
    Task<OperationResult<User>> CreateGuest(string playerName, IFormFile avatar, CancellationToken ct);
}