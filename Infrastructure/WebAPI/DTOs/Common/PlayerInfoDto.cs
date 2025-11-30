namespace Infrastructure.WebAPI.DTOs.Common
{
    public record PlayerInfoDto(
        Guid PlayerId,
        string Name,
        string AvatarUrl,
        bool IsReady, // Готов ли игрок к началу (можно добавить фичу "готовность")
        DateTime JoinedAt // Когда игрок присоединился (для отображения "только что")
    )
    {
        public PlayerInfoDto() : this(
            Guid.Empty, "", "", false, DateTime.UtcNow
        )
        { }
    };
}