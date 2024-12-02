using PowerPointService.Models;
using PowerPointService.Types;

namespace PowerPointService.Services;

public interface IPowerPointService
{
    Task<PresentationModel> SaveFileAsync(IFormFile file, CancellationToken cancellationToken);

    Task<VideosDto> GetVideosAsync(Guid id, CancellationToken cancellationToken);

    Task<VideoContentDto> GetVideoContentAsync(Guid id, CancellationToken cancellationToken);
}