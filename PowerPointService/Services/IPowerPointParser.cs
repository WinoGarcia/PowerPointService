using PowerPointService.Models;

namespace PowerPointService.Services;

public interface IPowerPointParser
{
    Task<IEnumerable<VideoModel>> ParseFileAsync(Guid presentationId, string fullFileName, CancellationToken cancellationToken);
}