using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IGameManager
{
    Task<OperationResult> StartNewGameAsync(Guid roomId, Guid startedByUserId, CancellationToken ct);
    Task<OperationResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan QuestionDuration, 
        CancellationToken ct,
        IEnumerable<Question>? questions = null);
    
    Task<OperationResult> SubmitAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer, CancellationToken ct);
    Task<OperationResult<Room>> FinishGameAsync(Guid userId, Guid roomId, CancellationToken ct);
}