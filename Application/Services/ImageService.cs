using Application.Interfaces;
using Domain.Common;
using Domain.Enums;
using Infrastructure.Interfaces;

namespace Application.Services;

public class ImageService(IImageRepository imageRepository) : IImageService
{
    public Task<OperationResult<FileStream>> GetAvatar(Guid id)
    {
        return imageRepository.GetImageFile(id, ImageType.Avatar);
    }

    public Task<OperationResult<FileStream>> GetQuestionImage(Guid id)
    {
        return imageRepository.GetImageFile(id, ImageType.Question);
    }

    public Task<OperationResult> SaveQuestionImages(IEnumerable<IFormFile> images)
    {
        throw new NotImplementedException();
    }
}