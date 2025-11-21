using Application.Interfaces;
using Application.Notifications;
using Application.Result;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

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

    public async Task<ServiceResult> StartNewGameAsync(Guid roomId, Guid startedByUserId)
    {
        if (!await CanStartGameAsync(roomId, startedByUserId))
            return ServiceResult.Error("Can't start new game");

        var room = await roomRepository.GetByIdAsync(roomId);
        if (room == null) 
            ServiceResult.Error("Room not found");

        var game = await roomRepository.GetCurrentGameAsync(roomId);
        if (game == null)
            ServiceResult.Error("Game not found");
        
        // TODO: оркестратор игры
        game.StartGame();
        
        logger.LogInformation($"Match {game.Id} started in room {roomId} by user {startedByUserId}");

        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan questionDuration,
        IEnumerable<Question>? questions = null)
    {
        var game = new Game(mode, questions, questionDuration);
        await roomRepository.AddGameAsync(game);
        
        var notification = new NewGameNotification(game);
        await notificationService.NotifyGameRoomAsync(roomId, notification);
        
        return ServiceResult<Game>.Ok(game);
    }

    public Task<ServiceResult> SubmitAnswerAsync(Guid roomId, Answer answer)
    {
        throw new NotImplementedException();
    }
}