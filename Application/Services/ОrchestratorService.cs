using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using Application.Notifications;
using Application.Result;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Services;

public class ОrchestratorService(Game game,
    Room room,
    INotificationService notificationService,
    IGameRepository gameRepository,
    IQuestionService questionService,
    IEvaluationService evaluationService): IОrchestratorService
{
    
    public async Task<OperationResult> RunGameCycle()
    {
        game.StartGame();
        var questionsRes = await questionService
            .GetAllQuestionsAsync(game);
        if (!questionsRes.Success)
            return OperationResult.Error(questionsRes.ErrorMsg);
        
        if (questionsRes.ResultObj is null)
            return OperationResult.Error(
                $"В репозиторий игры {game.Id} не были добавлены вопросы");
        
        var questions = questionsRes.ResultObj.ToList();
        game.CurrentStatistic = new Statistic();
        foreach (var question in questions)
        {
            await notificationService.NotifyGameRoomAsync(room.Id, 
                new NewQuestionNotification(question.Id,
                                            question.Formulation,
                                            DateTime.Now + game.QuestionDuration,
                                            game.QuestionDuration.Seconds));
            await Task.Delay(game.QuestionDuration);
            await notificationService.NotifyGameRoomAsync(room.Id,
                new QuestionClosedNotification(question.Id, question.RightAnswer.Date));
            var rawAnswer 
                = await gameRepository.LoadAnswersAsync(question.Id);
            if (rawAnswer.ResultObj is not null)
            {
                game.CurrentStatistic.Update(rawAnswer.ResultObj, question.RightAnswer.Date, 
                    evaluationService.CalculateScore(game.Mode));
                await gameRepository.SaveStatisticAsync(game.CurrentStatistic);
            }
            await notificationService.NotifyGameRoomAsync(room.Id,     
                new LeaderBoardNotifications(game.CurrentStatistic));
        }   
        game.FinishGame();  
        return OperationResult.Ok();
    }
}