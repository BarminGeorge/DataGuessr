using Application.Result;

namespace Application.Interfaces;

public interface IОrchestratorService
{
    public Task<OperationResult> RunGameCycle();
}