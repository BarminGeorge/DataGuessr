using Application.DtoUI;
using Application.Extensions;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Mappers;

public static class ModelToUiMapper
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto(
            room.Id, 
            room.Owner, 
            room.Players.Select(x => x.ToDto()),
            room.ClosedAt,
            room.InviteCode);
    }

    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Mode,
            game.Status,
            game.QuestionsCount,
            game.QuestionDuration.Seconds);
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.PlayerName,
            user.Avatar.GetUrl());
    }

    public static QuestionDto ToDto(this Question question, DateTime endTime, int durationSeconds)
    {
        return new QuestionDto(
            question.Id,
            question.Mode,
            question.Formulation,
            question.GetUrl(),
            endTime,
            durationSeconds);
    }

    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto(
            player.Id,
            player.UserId,
            player.Name,
            player.Avatar.GetUrl());
    }

    public static AnswerDto ToDto(this Answer answer)
    {
        return answer switch
        {
            DateTimeAnswer dateTimeAnswer => new DateTimeAnswerDto(dateTimeAnswer.Value.Year),
            BoolAnswer boolAnswer => new BoolAnswerDto(boolAnswer.Value ? 1 : 0),
            _ => throw new ArgumentException($"Unknown answer type: {answer.GetType().Name}")
        };
    }
}