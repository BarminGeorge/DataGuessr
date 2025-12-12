using System.Collections.ObjectModel;
using Application.DtoUI;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Mappers;

public static class ModelToUiMapper
{
    // TODO: переделать
    public static async Task<RoomDto> ToDto(this Room room, IUserRepository userRepository)
    {
        var players = new List<UserDto>();
        foreach (var player in room.Players)
        {
            var getUserOperation = () => userRepository.GetById(player.UserId);
            var userResult = await getUserOperation.WithRetry(delay: TimeSpan.FromSeconds(0.10));
            if (!userResult.Success || userResult.ResultObj is null)
                continue;
            players.Add(userResult.ResultObj.ToDto());
        }
        return new RoomDto(
            room.Id, 
            room.Owner, 
            players,
            room.ClosedAt);
    }

    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Mode,
            game.Status,
            new ReadOnlyCollection<QuestionDto>(
                game.Questions.Select(q => q.ToDto())
                              .ToList()),
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

    public static QuestionDto ToDto(this Question question)
    {
        return new QuestionDto(
            question.Mode,
            question.Formulation,
            question.ImageUrl);
    }
}