using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IGameManager
{
    Task<Game> StartNewGameAsync(Guid roomId, Guid startedByUserId);
    Task<Game> CreateNewGameAsync(Guid roomId, Guid startedByUserId, IMode gameMode);
}