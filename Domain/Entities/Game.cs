using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueTypes;
using Newtonsoft.Json;

namespace Domain.Entities;

public class Game : IEntity<Guid>
{
    // EF Core требует сеттеры
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }

    public GameMode Mode { get; private set; }
    public GameStatus Status { get; private set; }
    public int QuestionsCount { get; private set; }
    public TimeSpan QuestionDuration { get; private set; }

    // Сериализованная статистика (JSON)
    public string? StatisticJson { get; private set; }

    // Навигационное свойство для EF Core
    public virtual ICollection<Question> Questions { get; private set; } = new List<Question>();

    // Конструктор для EF Core
    protected Game() { }

    public Game(
        Guid roomId,
        GameMode mode,
        TimeSpan questionDuration,
        int questionsCount)
    {
        Id = Guid.NewGuid();
        RoomId = roomId;
        Mode = mode;
        Status = GameStatus.NotStarted;
        QuestionDuration = questionDuration;
        QuestionsCount = questionsCount;
        StatisticJson = null;
    }

    public void StartGame()
    {
        if (Status != GameStatus.NotStarted)
            throw new InvalidOperationException("Игра уже началась");

        Status = GameStatus.InProgress;
    }

    public void FinishGame()
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationException("Игра еще не началась или уже закончена");

        Status = GameStatus.Finished;
    }

    //Метод для СОХРАНЕНИЯ статистики
    public void SetStatistic(Statistic statistic)
    {
        StatisticJson = JsonConvert.SerializeObject(statistic);
    }

    //Метод для ПОЛУЧЕНИЯ статистики
    public Statistic? GetStatistic()
    {
        if (string.IsNullOrEmpty(StatisticJson))
            return null;

        return JsonConvert.DeserializeObject<Statistic>(StatisticJson);
    }
}
