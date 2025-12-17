using System.Text.Json.Serialization;

namespace Domain.ValueTypes;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DateTimeAnswer), typeDiscriminator: "datetime")]
[JsonDerivedType(typeof(BoolAnswer), typeDiscriminator: "bool")]
public abstract record Answer;

public record DateTimeAnswer(DateTime Value) : Answer;
public record BoolAnswer(bool Value) : Answer; 