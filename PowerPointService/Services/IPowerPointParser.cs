using PowerPointService.Types;

namespace PowerPointService.Services;

public interface IPowerPointParser
{
    Task<IEnumerable<VideoModel>> ParseFileAsync(Guid presentationId, FileStream fileStream, CancellationToken cancellationToken);
}