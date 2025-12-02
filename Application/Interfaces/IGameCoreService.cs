using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces;

public interface IGameCoreService
{
    public Task<OperationResult> RunGameCycle(Game game, Guid roomId, CancellationToken canсelToken);
}