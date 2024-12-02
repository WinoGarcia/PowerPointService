using Microsoft.Extensions.Options;
using PowerPointService.Models;
using PowerPointService.Types;

namespace PowerPointService.Services;

public sealed class PowerPointService : IPowerPointService
{
    #region Private Fields

    private readonly ILogger<PowerPointService> logger;
    private readonly IDatabaseRepository databaseRepository;
    private readonly SettingOptions options;
    private readonly IPowerPointParser powerPointParser;

    #endregion

    #region Constructors

    public PowerPointService(
        ILogger<PowerPointService> logger,
        IOptions<SettingOptions> options,
        IDatabaseRepository databaseRepository,
        IPowerPointParser powerPointParser)
    {
        this.logger = logger;
        this.databaseRepository = databaseRepository;
        this.options = options.Value;
        this.powerPointParser = powerPointParser;
    }

    #endregion

    #region Public Methods

    public async Task<PresentationModel> SaveFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var fullFileName = Path.Combine(this.options.PathBase, file.FileName);

            await using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            var presentationModel = new PresentationModel
            {
                Id = Guid.CreateVersion7(),
                Name = file.FileName,
                FullFileName = fullFileName,
                State = PresentationStates.Adding
            };

            var result = await this.databaseRepository.InsertPresentationAsync(presentationModel, cancellationToken);
            if (result == 0)
            {
                return presentationModel;
            }

            var _ = Task.Run(() => this.ExtractVideosAsync(presentationModel, CancellationToken.None), CancellationToken.None);

            return presentationModel;
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "fileName: {FileName}", file.FileName);
            return null;
        }
    }

    public async Task<VideosDto> GetVideosAsync(Guid id, CancellationToken cancellationToken)
    {
        var videos = await this.databaseRepository.GetVideosWithPresentationAsync(id, cancellationToken);

        return Map(videos);
    }

    public async Task<VideoContentDto> GetVideoContentAsync(Guid id, CancellationToken cancellationToken) =>
        await this.databaseRepository.GetVideoAsync(id, cancellationToken);

    #endregion

    #region Private Methods

    private async Task ExtractVideosAsync(PresentationModel presentation, CancellationToken cancellationToken)
    {
        try
        {
            var videoModels = await this.powerPointParser.ParseFileAsync(presentation.Id, presentation.FullFileName, cancellationToken);
            if (videoModels.Any())
            {
                await this.databaseRepository.InsertVideosAsync(videoModels, cancellationToken);
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "presentationId: {PresentationId} fullFileName: {FullFileName}",
                presentation.Id,
                presentation.FullFileName);
        }
        finally
        {
            await this.databaseRepository.UpdatePresentationStateAsync(presentation.Id, PresentationStates.Added, cancellationToken);
        }
    }

    private static VideosDto Map(IEnumerable<VideosWithPresentation> videos)
    {
        if (videos.Any())
        {
            var videosDto = new VideosDto();
            var presentationModel = videos.First().MapToPresentation();
            videosDto.Presentation = presentationModel;

            var groupBySlideIds = videos.GroupBy(v => v.SlideId);
            foreach (var groupBySlideId in groupBySlideIds)
            {
                var grouped = new GroupedBySlide
                {
                    SlideId = groupBySlideId.Key,
                    Videos = []
                };

                foreach (var video in groupBySlideId)
                {
                    grouped.Videos.Add(video.MapToVideo());
                }

                videosDto.GroupedVideos.Add(grouped);
            }

            return videosDto;
        }

        return null;
    }

    #endregion
}