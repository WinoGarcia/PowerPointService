using PowerPointService.Models;
using PowerPointService.Types;

namespace PowerPointService.Services;

public interface IDatabaseRepository
{
    Task<int> InsertPresentationAsync(PresentationModel presentationModel, CancellationToken cancellationToken);

    Task<int> UpdatePresentationStateAsync(Guid presentationId, PresentationStates state, CancellationToken cancellationToken);

    Task<int> InsertVideosAsync(IEnumerable<VideoModel> videoModels, CancellationToken cancellationToken);
    Task<IEnumerable<VideosWithPresentation>> GetVideosWithPresentationAsync(Guid id, CancellationToken cancellationToken);
    Task<VideoContentDto> GetVideoAsync(Guid id, CancellationToken cancellationToken);
}