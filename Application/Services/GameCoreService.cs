using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Notifications;
using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Services;

public class GameCoreService(
    Game game,
    Room room,
    INotificationService notificationService,
    IGameRepository gameRepository,
    IQuestionService questionService,
    IEvaluationService evaluationService)
    : IGameCoreService
{
    
    public async Task<OperationResult> RunGameCycle(CancellationToken token)
    {
        game.StartGame();
        var questionsRes = await questionService
            .GetAllQuestionsAsync(game, token);
        if (!questionsRes.Success)
            return OperationResult.Error(questionsRes.ErrorMsg);
        
        if (questionsRes.ResultObj is null)
            return OperationResult.Error($"В репозиторий игры {game.Id} не были добавлены вопросы");
        
        var questions = questionsRes.ResultObj.ToList();
        game.CurrentStatistic = new Statistic();
        foreach (var question in questions)
        {
            await notificationService.NotifyGameRoomAsync(room.Id, 
                new NewQuestionNotification(question.Id,
                                            question.Formulation,
                                            question.ImageUrl,
                                            DateTime.Now + game.QuestionDuration,
                                            game.QuestionDuration.Seconds));
            await Task.Delay(game.QuestionDuration);
            await notificationService.NotifyGameRoomAsync(room.Id,
                new QuestionClosedNotification(question.Id, question.RightAnswer.Date));
            var rawAnswer = await gameRepository.LoadAnswersAsync(question.Id, token);
            // TODO: ошибки
            if (rawAnswer.ResultObj is not null)
            {
                game.CurrentStatistic.Update(rawAnswer.ResultObj, question.RightAnswer.Date, 
                    evaluationService.CalculateScore(game.Mode));
                await gameRepository.SaveStatisticAsync(game.CurrentStatistic, token);
            }
            await notificationService.NotifyGameRoomAsync(room.Id,     
                new StatisticNotification(game.CurrentStatistic));
        }   
        game.FinishGame();  
        return OperationResult.Ok();
    }
}