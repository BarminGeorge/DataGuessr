using Application.Interfaces;
using Application.Notifications;
using Domain.Builders;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class GameManager(
    IRoomRepository roomRepository, 
    INotificationService notificationService,
    ILogger<GameManager> logger)
    : IGameManager
{
    private async Task<bool> CanStartGameAsync(Guid roomId, Guid userId)
    {
        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            return false;

        if (room.Host != userId) 
            return false;

        return room.Players.Count >= 2;
    }

    public async Task<Game> CreateNewGameAsync(Guid roomId, Guid startedByUserId, IMode gameMode)
    {
        var game = new Game(gameMode);
        return await roomRepository.AddGameAsync(game);
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

        logger.LogInformation($"Match {game.Id} started in room {roomId} by user {startedByUserId}");

        return game;
    }
}