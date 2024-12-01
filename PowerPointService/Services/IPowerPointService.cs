using PowerPointService.Types;

namespace PowerPointService.Services;

public interface IPowerPointService
{
    Task<PresentationModel> SaveFileAsync(IFormFile file, CancellationToken cancellationToken);
}