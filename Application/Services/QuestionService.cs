using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Services;

public class QuestionService(IQuestionRepository questionRepository) : IQuestionService
{
    public async Task<IEnumerable<Question>> GetAllQuestionsAsync(Game game)
    {
        if (game.Questions.Count > 0)
            return game.Questions;
        
        var count = game.Questions.Count;
        return await questionRepository.GetQuestionsAsync(count);
    }

    public Task SubmitAnswerAsync(Guid questionId, Answer answer)
    {
        throw new NotImplementedException();
    }
}