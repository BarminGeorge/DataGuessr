using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Services;

public class QuestionService(IQuestionRepository questionRepository, IGameRepository gameRepository) : IQuestionService
{
    public async Task<OperationResult<IEnumerable<Question>>> GetAllQuestionsAsync(Game game,  CancellationToken ct)
    {
        if (game.Questions.Count > 0)
            return OperationResult<IEnumerable<Question>>.Ok(game.Questions);
        
        var count = game.Questions.Count;
        return await questionRepository.GetUniqQuestionsAsync(count, ct);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid questionId, Answer answer,  CancellationToken ct)
    {
        return await gameRepository.SaveAnswerAsync(roomId, gameId, questionId, answer, ct);
    }
}