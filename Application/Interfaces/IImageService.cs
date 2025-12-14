using Domain.Common;

namespace Application.Interfaces;

public interface IImageService
{
    Task<OperationResult<FileStream>> GetAvatar(Guid id);
    Task<OperationResult<FileStream>> GetQuestionImage(Guid id);
}