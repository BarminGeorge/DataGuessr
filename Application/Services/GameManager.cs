using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Notifications;
using Application.Result;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Services;

public class GameManager(
    IОrchestratorService оrchestratorService,
    IRoomRepository roomRepository, 
    INotificationService notificationService,
    IQuestionService questionService,
    ILogger<GameManager> logger)
    : IGameManager
{
    private static bool IsOwnerRoom(Room? room, Guid userId)
    {
        if (room == null) 
            return false;

        return room.Owner == userId;
    }
    
    private async Task<OperationResult> CanStartGameAsync(Guid roomId, Guid userId)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId);
        if (!getRoomResult.Success)
            return OperationResult.Error(getRoomResult.ErrorMsg);
        var room = getRoomResult.ResultObj;

        var canStart = IsOwnerRoom(room, userId) && room.Players.Count >= 2;
        var errMessage = canStart ? "" : "You are not owner or players count less then 2";
        return new OperationResult(canStart, errMessage);
    }

    public async Task<OperationResult> StartNewGameAsync(Guid roomId, Guid startedByUserId)
    {
        var canStartGame = await CanStartGameAsync(roomId, startedByUserId);
        if (!canStartGame.Success)
            return OperationResult.Error($"Can't start new game: {canStartGame.ErrorMsg}");
        
        var getRoomResult = await roomRepository.GetByIdAsync(roomId);
        if (!getRoomResult.Success) 
            return OperationResult.Error(getRoomResult.ErrorMsg);

        var getGameResult = await roomRepository.GetCurrentGameAsync(roomId);
        if (!getGameResult.Success)
            return OperationResult.Error(getGameResult.ErrorMsg);

        var game = getGameResult.ResultObj;
        
        await оrchestratorService.RunGameCycle();
        logger.LogInformation($"Match {game.Id} started in room {roomId} by user {startedByUserId}");
        
        return OperationResult.Ok();
    }

    public async Task<OperationResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan questionDuration,
        IEnumerable<Question>? questions = null)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId);
        if (!getRoomResult.Success)
            return OperationResult<Game>.Error(getRoomResult.ErrorMsg);

        var room = getRoomResult.ResultObj;
        if (!IsOwnerRoom(room, createdByUserId))
            OperationResult.Error("Can't create new game, you are not the owner");
        
        var game = new Game(mode, questions, questionDuration, countQuestions);
        var addGameResult = await roomRepository.AddGameAsync(game);
        if (!addGameResult.Success)
            return OperationResult<Game>.Error(addGameResult.ErrorMsg);
        
        var notification = new NewGameNotification(game);
        var notifyResult = await notificationService.NotifyGameRoomAsync(roomId, notification);
        return !notifyResult.Success 
            ? OperationResult<Game>.Error(notifyResult.ErrorMsg) 
            : OperationResult<Game>.Ok(game);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer)
    {
        return await questionService.SubmitAnswerAsync(roomId, gameId, questionId, answer);
    }
}