using Application.Result;

namespace Application.Interfaces;

public interface IGameCoreService
{
    public Task<OperationResult> RunGameCycle(CancellationToken token);
}