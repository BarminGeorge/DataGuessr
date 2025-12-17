using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;

namespace Application.Services;

public class QuestionService(
    IQuestionRepository questionRepository,
    IPlayerAnswerRepository answersRepository) : IQuestionService
{
    public async Task<OperationResult<IEnumerable<Question>>> GetAllQuestionsAsync(Game game, CancellationToken ct)
    {
        if (game.Questions.Count > 0)
            return OperationResult<IEnumerable<Question>>.Ok(game.Questions);

        var count = game.QuestionsCount;
        return await questionRepository.GetUniqQuestionsAsync(count, game.Mode, ct);
    }

    public async Task<OperationResult> SubmitAnswerAsync(Guid gameId, Guid questionId, Guid playerId, Answer answer, CancellationToken ct)
    {
        return await answersRepository.SaveAnswerAsync(gameId, questionId, playerId, answer, ct);
    }
}