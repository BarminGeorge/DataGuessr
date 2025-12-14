using Domain.Entities;

namespace Application.Extensions;

public static class ImageUrlExtensions
{
    public static string GetUrl(this Avatar avatar)
    {
        return $"/api/avatars/{avatar.Id}";
    }

    public static string GetUrl(this Question question)
    {
        return $"/api/questions/{question.Id}";
    }
}