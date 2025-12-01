using Application.Interfaces;
using Application.Notifications;
using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;

namespace Application.Services;

public class GameCoreService(
    Game game,
    Guid roomId,
    INotificationService notificationService,
    IGameRepository gameRepository,
    IQuestionService questionService,
    IEvaluationService evaluationService,
    IPlayerAnswerRepository answerRepository)
    : IGameCoreService
{
    
    public async Task<OperationResult> RunGameCycle(CancellationToken canсelToken)
    {
        game.StartGame();
        var questionsRes = await questionService
            .GetAllQuestionsAsync(game, canсelToken);
        if (!questionsRes.Success)
            return OperationResult.Error(questionsRes.ErrorMsg);
        
        if (questionsRes.ResultObj is null)
            return OperationResult.Error($"В репозиторий игры {game.Id} не были добавлены вопросы");
        
        var questions = questionsRes.ResultObj.ToList();
        game.CurrentStatistic = new Statistic();
        foreach (var question in questions)
        {
            await notificationService.NotifyGameRoomAsync(roomId, 
                new NewQuestionNotification(question.Id,
                                            question.Formulation,
                                            question.ImageUrl,
                                            DateTime.Now + game.QuestionDuration,
                                            game.QuestionDuration.Seconds));
            await Task.Delay(game.QuestionDuration, canсelToken);
            await notificationService.NotifyGameRoomAsync(roomId,
                new QuestionClosedNotification(question.Id, question.RightAnswer.Date));
            var rawAnswer = await answerRepository.LoadAnswersAsync(game.Id, question.Id, canсelToken);
            if (!rawAnswer.Success) return OperationResult.Error(rawAnswer.ErrorMsg);
            if (rawAnswer.ResultObj is not null)
            {
                game.CurrentStatistic.Update(rawAnswer.ResultObj, question.RightAnswer.Date, 
                    evaluationService.CalculateScore(game.Mode));
                var saveStatisticResult = await gameRepository.SaveStatisticAsync(game.Id, game.CurrentStatistic, canсelToken);
                if (!saveStatisticResult.Success) return OperationResult.Error(saveStatisticResult.ErrorMsg);
            }
            await notificationService.NotifyGameRoomAsync(roomId,     
                new StatisticNotification(game.CurrentStatistic));
        }   
        game.FinishGame();  
        return OperationResult.Ok();
    }
}