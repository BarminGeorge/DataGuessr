using System.Text.Json.Serialization;

namespace Domain.ValueTypes;

public abstract record Answer;
public record DateTimeAnswer(DateTime Value) : Answer;
public record BoolAnswer(bool Value) : Answer;