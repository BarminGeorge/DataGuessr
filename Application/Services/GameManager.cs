using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Notifications;
using Application.Result;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Services;

public class GameManager(
    IGameCoreService gameCoreService,
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
    
    private async Task<OperationResult> CanStartGameAsync(Guid roomId, Guid userId,  CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success)
            return OperationResult.Error(getRoomResult.ErrorMsg);
        var room = getRoomResult.ResultObj;

        var canStart = IsOwnerRoom(room, userId) && room.Players.Count >= 2;
        var errMessage = canStart ? "" : "You are not owner or players count less then 2";
        return new OperationResult(canStart, errMessage);
    }

    public async Task<OperationResult> StartNewGameAsync(Guid roomId, Guid startedByUserId, CancellationToken ct)
    {
        var canStartGame = await CanStartGameAsync(roomId, startedByUserId, ct);
        if (!canStartGame.Success)
            return OperationResult.Error($"Can't start new game: {canStartGame.ErrorMsg}");
        
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success) 
            return OperationResult.Error(getRoomResult.ErrorMsg);

        var getGameResult = await roomRepository.GetCurrentGameAsync(roomId, ct);
        if (!getGameResult.Success)
            return OperationResult.Error(getGameResult.ErrorMsg);

        var game = getGameResult.ResultObj;
        
        await gameCoreService.RunGameCycle(ct);
        logger.LogInformation($"Match {game.Id} started in room {roomId} by user {startedByUserId}");
        
        return OperationResult.Ok();
    }

    public async Task<OperationResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan questionDuration,
        CancellationToken ct,
        IEnumerable<Question>? questions = null)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success)
            return OperationResult<Game>.Error(getRoomResult.ErrorMsg);

        var room = getRoomResult.ResultObj;
        if (!IsOwnerRoom(room, createdByUserId))
            OperationResult.Error("Can't create new game, you are not the owner");
        
        var game = new Game(mode, questions, questionDuration, countQuestions);
        var addGameResult = await roomRepository.AddGameAsync(game, ct);
        if (!addGameResult.Success)
            return OperationResult<Game>.Error(addGameResult.ErrorMsg);
        
        room.AddGame(game);
        var resultUpdate = await roomRepository.UpdateAsync(room, ct);
        if (!resultUpdate.Success)
            return OperationResult<Game>.Error(resultUpdate.ErrorMsg);
        
        var notification = new NewGameNotification(game);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
        
        return !notifyResult.Success 
            ? OperationResult<Game>.Error(notifyResult.ErrorMsg) 
            : OperationResult<Game>.Ok(game);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer, CancellationToken ct)
    {
        return await questionService.SubmitAnswerAsync(roomId, gameId, questionId, answer, ct);
    }

    public async Task<OperationResult<Room>> FinishGameAsync(Guid userId, Guid roomId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success)
            return OperationResult<Room>.Error(getRoomResult.ErrorMsg);

        if (!IsOwnerRoom(getRoomResult.ResultObj, userId))
            return OperationResult<Room>.Error("Чтобы закончить игру нужно быть её создателем");

        var notification = new ReturnToRoomNotification(getRoomResult.ResultObj);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));

        return notifyResult.Success
            ? OperationResult<Room>.Ok(getRoomResult.ResultObj)
            : OperationResult<Room>.Error(notifyResult.ErrorMsg);
    }
}