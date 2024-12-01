using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Extensions.Options;
using PowerPointService.Helpers;
using PowerPointService.Models;
using PowerPointService.Types;

namespace PowerPointService.Services;

public class PowerPointParser : IPowerPointParser
{
    #region Private Fields

    private readonly SettingOptions options;
    private readonly ILogger<PowerPointParser> logger;
    private readonly IDatabaseRepository databaseRepository;
    private readonly IFFMpegService ffMpegProvider;

    #endregion

    #region Constructors

    public PowerPointParser(
        ILogger<PowerPointParser> logger,
        IOptions<SettingOptions> options,
        IDatabaseRepository databaseRepository,
        IFFMpegService ffMpegProvider)
    {
        this.logger = logger;
        this.databaseRepository = databaseRepository;
        this.ffMpegProvider = ffMpegProvider;
        this.options = options.Value;
    }

    #endregion

    #region Public Methods

    public async Task<IEnumerable<VideoModel>> ParseFileAsync(Guid presentationId, string fullFileName, CancellationToken cancellationToken)
    {
        var videoModels = new List<VideoModel>();

        try
        {
            using var presentation = PresentationDocument.Open(fullFileName, false);
            if (presentation.PresentationPart is null)
            {
                return videoModels;
            }

            var slideIdList = presentation.PresentationPart.Presentation.SlideIdList;
            if (slideIdList is null)
            {
                return videoModels;
            }

            var slideCount = 0;
            foreach (var slideId in slideIdList.Elements<SlideId>())
            {
                if (slideId.RelationshipId is not null)
                {
                    var slidePart = presentation.PresentationPart.GetPartById(slideId.RelationshipId);

                    var videoReferenceRelationship = slidePart.DataPartReferenceRelationships.Where(d => d is VideoReferenceRelationship);
                    foreach (var videoReference in videoReferenceRelationship)
                    {
                        if (videoReference.DataPart is MediaDataPart mediaDataPart)
                        {
                            var videoModel = await this.SaveVideoAsync(presentationId, slideCount, mediaDataPart, cancellationToken);
                            videoModels.Add(videoModel);
                        }
                    }
                }

                slideCount++;
            }

            if (videoModels.Any())
            {
                var result = await this.databaseRepository.InsertVideosAsync(videoModels, cancellationToken);
                if (result != 0)
                {
                    await this.databaseRepository.UpdatePresentationStateAsync(presentationId, PresentationStates.Added, cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "presentationId: {PresentationId} fullFileName: {FullFileName}",
                presentationId,
                fullFileName);
        }

        return videoModels;
    }

    #endregion

    #region Private Methods

    private async Task<VideoModel> SaveVideoAsync(
        Guid presentationId,
        int slideCount,
        MediaDataPart mediaDataPart,
        CancellationToken cancellationToken)
    {
        var fileId = Guid.CreateVersion7();
        var fileName = $"v_{slideCount}_{fileId}{mediaDataPart.MapExtension()}";
        var fullVideoFileName = Path.Combine(this.options.PathBase, this.options.VideoPath, fileName);

        await using (var videoStream = mediaDataPart.GetStream())
        await using (var videoFileStream = new FileStream(fullVideoFileName, FileMode.Create, FileAccess.Write))
        {
            await videoStream.CopyToAsync(videoFileStream, cancellationToken);
        }

        var mediaInfo = await this.ffMpegProvider.GetMediaInfoAsync(fullVideoFileName, cancellationToken);
        return new VideoModel
        {
            PresentationId = presentationId,
            SlideId = slideCount,
            Id = fileId,
            Name = fileName,
            FullFileName = fullVideoFileName,
            Duration = mediaInfo.Duration
        };
    }

    #endregion
}