using Application.Result;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IGameManager
{
    Task<OperationResult> StartNewGameAsync(Guid roomId, Guid startedByUserId);
    Task<OperationResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan QuestionDuration, 
        IEnumerable<Question> questions = null);
    
    Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer);
}