namespace Domain.ValueTypes;

public class JwtOptions
{
    public string SecretKey { get; set; }
    public int ExpireInHours { get; set; } = 12;
}