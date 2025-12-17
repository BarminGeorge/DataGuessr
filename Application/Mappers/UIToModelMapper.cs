using Application.DtoUI;
using Domain.ValueTypes;

namespace Application.Mappers;

public static class UiToModelMapper
{
    public static Answer ToAnswer(this AnswerDto dto)
    {
        return dto switch
        {
            DateTimeAnswerDto dateTimeDto => new DateTimeAnswer(new DateTime(dateTimeDto.Year, 1, 1)),
            BoolAnswerDto boolDto => new BoolAnswer(boolDto.NumericValue == 1),
            _ => throw new ArgumentException($"Unknown answer type: {dto.GetType().Name}")
        };
    }
}