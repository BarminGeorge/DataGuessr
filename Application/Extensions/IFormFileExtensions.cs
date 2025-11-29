namespace Application.Extensions;

public static class FormFileExtensions
{
    public static bool IsValid(this IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
            return false;

        if (file.Length > 5 * 1024 * 1024)
            return false;

        return true;
    }
}