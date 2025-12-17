using System.Text.Json.Serialization;

namespace Application.DtoUI;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DateTimeAnswerDto), typeDiscriminator: "datetime")]
[JsonDerivedType(typeof(BoolAnswerDto), typeDiscriminator: "bool")]
public abstract record AnswerDto;

public record DateTimeAnswerDto([property: JsonPropertyName("value")] int Year) : AnswerDto
{
    public DateTimeAnswerDto(DateTime dateTime) : this(dateTime.Year)
    {
    }
}

public record BoolAnswerDto([property: JsonPropertyName("value")] int NumericValue) : AnswerDto
{
    public BoolAnswerDto(bool boolValue) : this(boolValue ? 1 : 0)
    {
    }
}