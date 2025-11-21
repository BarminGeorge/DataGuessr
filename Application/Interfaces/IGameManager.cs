using Application.Result;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IGameManager
{
    Task<ServiceResult> StartNewGameAsync(Guid roomId, Guid startedByUserId);
    Task<ServiceResult<Game>> CreateNewGameAsync(Guid roomId, 
        Guid createdByUserId, 
        GameMode mode, 
        int countQuestions, 
        TimeSpan QuestionDuration, 
        IEnumerable<Question> questions = null);
    
    Task<ServiceResult> SubmitAnswerAsync(Guid roomId, Answer answer);
}