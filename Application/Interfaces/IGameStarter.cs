using Domain.Entities;

namespace Application.Interfaces;

public interface IGameStarter
{
    Task<Game> StartNewGameAsync(Guid roomId, Guid startedByUserId);
    Task<bool> CanStartGameAsync(Guid roomId, Guid userId);
}