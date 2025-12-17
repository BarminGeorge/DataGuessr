namespace Application.DtoUI;

public abstract record AnswerDto;

public record DateTimeAnswerDto(int Year) : AnswerDto;

public record BoolAnswerDto(int NumericValue) : AnswerDto
{
    public BoolAnswerDto(bool boolValue) : this(boolValue ? 1 : 0)
    {
    }
}