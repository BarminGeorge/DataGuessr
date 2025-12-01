using Domain.Common;

namespace Application.Interfaces;

public interface IGameCoreService
{
    public Task<OperationResult> RunGameCycle(CancellationToken canсelToken);
}