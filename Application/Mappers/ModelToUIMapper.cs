using Application.DtoUI;
using Domain.Entities;

namespace Application.Mappers;

public static class ModelToUiMapper
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto(
            room.Id, 
            room.Owner, 
            room.Players.ToList(),
            room.ClosedAt);
    }

    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Mode,
            game.Status,
            game.CurrentStatistic,
            game.Questions.ToList(),
            game.QuestionsCount,
            game.QuestionDuration);
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.PlayerName,
            user.Avatar.Filename);
    }
}