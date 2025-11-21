using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Result;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Services;

public class QuestionService(IQuestionRepository questionRepository, IGameRepository gameRepository) : IQuestionService
{
    public async Task<OperationResult<IEnumerable<Question>>> GetAllQuestionsAsync(Game game)
    {
        if (game.Questions.Count > 0)
            return OperationResult<IEnumerable<Question>>.Ok(game.Questions);
        
        var count = game.Questions.Count;
        var getQuestionsResult =  await questionRepository.GetQuestionsAsync(count);
        return getQuestionsResult.Success 
            ? OperationResult<IEnumerable<Question>>.Ok(getQuestionsResult.ResultObj) 
            : OperationResult<IEnumerable<Question>>.Error(getQuestionsResult.ErrorMsg);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer)
    {
        var saveAnswerResult = await gameRepository.SaveAnswerAsync(roomId, gameId, questionId, answer);
        return saveAnswerResult.Success
            ? OperationResult.Ok()
            : OperationResult.Error(saveAnswerResult.ErrorMsg);
    }
}