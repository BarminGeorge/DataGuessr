using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Result;

namespace Application.Services;

public class ConnectionService(IConnectionRepository connectionRepository) : IConnectionService
{
    public async Task<OperationResult> AddConnection(string connectionId, Guid userId, Guid roomId)
    {
        return await connectionRepository.AddConnection(connectionId, userId, roomId);
    }

    public async Task<OperationResult> RemoveConnection(string connectionId)
    {
        return await connectionRepository.RemoveConnection(connectionId);
    }

    public async Task<OperationResult<(Guid userId, Guid roomId)>> GetUserByConnection(string connectionId)
    {
        return await connectionRepository.GetUserByConnectionId(connectionId);
    }
}