using Application.Interfaces;
using Application.Mappers;
using Application.Notifications;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;
using Infrastructure.Interfaces;

namespace Application.Services;

public class GameManager(
    IRoomRepository roomRepository, 
    INotificationService notificationService,
    IQuestionService questionService,
    IGameRepository gameRepository,
    IServiceScopeFactory scopeFactory)
    : IGameManager
{
    private static bool IsOwnerRoom(Room room, Guid userId) => room.Owner == userId;
    
    private async Task<OperationResult> CanStartGameAsync(Guid roomId, Guid userId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null)
            return getRoomResult;
        
        var room = getRoomResult.ResultObj;

        var canStart = IsOwnerRoom(room, userId) && room.Players.Count >= 2;
        var errMessage = canStart ? "" : "You are not owner or players count less then 2";
        var errType = canStart ? ErrorType.None : ErrorType.InvalidOperation;
        return new OperationResult(canStart, errMessage, errType);
    }

    public async Task<OperationResult> StartNewGameAsync(Guid roomId, Guid startedByUserId, CancellationToken ct)
    {
        var canStartGame = await CanStartGameAsync(roomId, startedByUserId, ct);
        if (!canStartGame.Success)
            return canStartGame;
            
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success) 
            return getRoomResult;

        var getGameResult = await roomRepository.GetCurrentGameAsync(roomId, ct);
        if (!getGameResult.Success || getGameResult.ResultObj == null)
            return getGameResult;

        var game = getGameResult.ResultObj;

        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();
            try
            {
                var gameCoreService = scope.ServiceProvider.GetRequiredService<IGameCoreService>();
                await gameCoreService.RunGameCycle(game, roomId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Game cycle failed for room {roomId}: {ex}");
            }
        }, ct);
        
        return OperationResult.Ok();
    }

    public async Task<OperationResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan questionDuration,
        CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null)
            return getRoomResult.ConvertToOperationResult<Game>();

        var room = getRoomResult.ResultObj;
        if (!IsOwnerRoom(room, createdByUserId))
            return OperationResult<Game>.Error.InvalidOperation("Can't create new game, you are not the owner");
            
        var game = new Game(roomId, mode, questionDuration, countQuestions);
        
        var addGameResult = await gameRepository.AddGameAsync(game, ct);
        if (!addGameResult.Success)
            return addGameResult.ConvertToOperationResult<Game>();
        
        room.AddGame(game);
        var resultUpdate = await roomRepository.UpdateAsync(room, ct);
        if (!resultUpdate.Success)
            return resultUpdate.ConvertToOperationResult<Game>();
        
        var notification = new NewGameNotification(game.ToDto());
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));
        
        return !notifyResult.Success 
            ? notifyResult.ConvertToOperationResult<Game>()
            : OperationResult<Game>.Ok(game);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer, CancellationToken ct)
    {
        return await questionService.SubmitAnswerAsync(gameId, questionId, playerId, answer, ct);
    }

    public async Task<OperationResult<Room>> FinishGameAsync(Guid userId, Guid roomId, CancellationToken ct)
    {
        var getRoomResult = await roomRepository.GetByIdAsync(roomId, ct);
        if (!getRoomResult.Success || getRoomResult.ResultObj == null)
            return getRoomResult;

        if (!IsOwnerRoom(getRoomResult.ResultObj, userId))
            return OperationResult<Room>.Error.InvalidOperation("Can't finish game, you are not the owner");
            
        var notification = new ReturnToRoomNotification(getRoomResult.ResultObj.ToDto());
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, notification);
        var notifyResult = await operation.WithRetry(delay: TimeSpan.FromSeconds(0.15));

        return notifyResult.Success
            ? OperationResult<Room>.Ok(getRoomResult.ResultObj)
            : notifyResult.ConvertToOperationResult<Room>();
    }
}