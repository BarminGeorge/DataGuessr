using Application.Interfaces;
using Domain.Builders;
using Domain.Entities;

namespace Application.Services;

public class MatchStarter(IRoomRepository roomRepository, ILogger<MatchStarter> logger)
    : IGameStarter
{
    public async Task<bool> CanStartGameAsync(Guid roomId, Guid userId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            return false;

        if (room.Host != userId) 
            return false;

        return room.Players.Count >= 2;
    }

    public async Task<Game> StartNewGameAsync(Guid roomId, Guid startedByUserId)
    {
        if (!await CanStartGameAsync(roomId, startedByUserId))
        {
            throw new InvalidOperationException("Cannot start match");
        }

        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            throw new ArgumentException("Room not found");

        var game = GameBuilder.Create().WithDefaultMode();
        room.AddGame(game);
        await roomRepository.UpdateAsync(room);

        logger.LogInformation("Match {MatchId} started in room {RoomId} by user {UserId}", 
            game.Id, roomId, startedByUserId);

        return game;
    }
}