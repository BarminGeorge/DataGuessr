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
        return await questionRepository.GetQuestionsAsync(count);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer)
    {
        return await gameRepository.SaveAnswerAsync(roomId, gameId, questionId, answer);
    }
}