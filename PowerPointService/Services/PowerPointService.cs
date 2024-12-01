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

            await using var fileStream = new FileStream(fullFileName, FileMode.Create);

            await file.CopyToAsync(fileStream, cancellationToken);
            var presentationModel = new PresentationModel
            {
                Id = Guid.NewGuid(),
                Name = file.FileName,
                FullFileName = fullFileName,
                State = PresentationStates.Adding
            };

            var result = await this.databaseRepository.InsertPresentationAsync(presentationModel, cancellationToken);
            if (result == 0)
            {
                return presentationModel;
            }

            var videoModels = await this.powerPointParser.ParseFileAsync(presentationModel.Id, fileStream, cancellationToken);
            if (videoModels.Any())
            {
                result = await this.databaseRepository.InsertVideosAsync(videoModels, cancellationToken);
                if (result != 0)
                {
                    result = await this.databaseRepository.UpdatePresentationStateAsync(presentationModel.Id, PresentationStates.Added, cancellationToken);
                    if (result != 0)
                    {
                        presentationModel.State = PresentationStates.Added;
                    }
                }
            }

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

        return this.Map(videos);
    }

    public async Task<VideoContentDto> GetVideoContentAsync(Guid id, CancellationToken cancellationToken) => 
        await this.databaseRepository.GetVideoAsync(id, cancellationToken);

    #endregion

    #region Private Methods

    private VideosDto Map(IEnumerable<VideosWithPresentation> videos)
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

                videosDto.Videos.Add(grouped);
            }

            return videosDto;
        }

        return null;
    }

    #endregion
}