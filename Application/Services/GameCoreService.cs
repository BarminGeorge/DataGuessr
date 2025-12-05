using Application.Interfaces;
using Application.Notifications;
using Domain.Common;
using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;

namespace Application.Services;

public class GameCoreService(
    INotificationService notificationService,
    IQuestionService questionService,
    IEvaluationService evaluationService,
    IPlayerAnswerRepository answerRepository)
    : IGameCoreService
{
    
    public async Task<OperationResult> RunGameCycle(Game game, Guid roomId, CancellationToken ct)
    {
        game.StartGame();
        
        var getQuestionsResult = await questionService.GetAllQuestionsAsync(game, ct);
        if (!getQuestionsResult.Success || getQuestionsResult.ResultObj == null)
            return getQuestionsResult;
        
        game.CurrentStatistic = new Statistic();
        
        foreach (var question in getQuestionsResult.ResultObj)
        {
            await NotifyRoomAboutNewQuestion(question, game, roomId);
            await Task.Delay(game.QuestionDuration, ct);
            await NotifyRoomAboutCloseQuestion(question, roomId);
            
            var rawAnswer = await answerRepository.LoadAnswersAsync(game.Id, question.Id, ct);
            if (!rawAnswer.Success || rawAnswer.ResultObj == null) 
                return rawAnswer;

            var oldStatistic = game.CurrentStatistic.Copy();
            UpdateStatistic(game, question, rawAnswer.ResultObj);
            var diff = game.CurrentStatistic.Diff(oldStatistic);
            
            await NotifyRoomAboutResults(diff, roomId);
            await NotifyRoomAboutResults(game.CurrentStatistic, roomId);
        }
        
        game.FinishGame();  
        return OperationResult.Ok();
    }

    private async Task NotifyRoomAboutNewQuestion(Question question, Game game, Guid roomId)
    {
        var newQuestionNotification = new NewQuestionNotification(question.Id,
                                                                  question.Formulation,
                                                                  question.ImageUrl,
                                                            DateTime.Now + game.QuestionDuration,
                                                                  game.QuestionDuration.Seconds);
        
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, newQuestionNotification);
        await operation.WithRetry(delay: TimeSpan.FromSeconds(0.2));
    }

    private async Task NotifyRoomAboutCloseQuestion(Question question, Guid roomId)
    {
        var closedQuestionNotification = new QuestionClosedNotification(question.Id, question.RightAnswer);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId,closedQuestionNotification);
        await operation.WithRetry(delay: TimeSpan.FromSeconds(0.2));
    }

    private void UpdateStatistic(Game game, Question question, Dictionary<Guid, Answer> answers)
    {
        var updateFunction = evaluationService.CalculateScore(game.Mode);
        game.CurrentStatistic?.Update(answers, question.RightAnswer, updateFunction);
    }

    private async Task NotifyRoomAboutResults(Statistic statistic, Guid roomId)
    {
        var statisticNotification = new StatisticNotification(statistic);
        var operation = () => notificationService.NotifyGameRoomAsync(roomId, statisticNotification);
        await operation.WithRetry(delay: TimeSpan.FromSeconds(0.2));
    }
}