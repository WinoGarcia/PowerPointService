using Microsoft.Extensions.Options;
using PowerPointService.Types;

namespace PowerPointService.Services;

public sealed class PowerPointService : IPowerPointService
{
    #region Private Fields

    private readonly ILogger<PowerPointService> logger;
    private readonly SettingOptions options;
    private readonly IPowerPointParser powerPointParser;

    #endregion

    #region Constructors

    public PowerPointService(
        ILogger<PowerPointService> logger,
        IOptions<SettingOptions> options,
        IPowerPointParser powerPointParser)
    {
        this.logger = logger;
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
                Id = Guid.CreateVersion7(),
                Name = file.FileName,
                FullFileName = fullFileName,
                State = PresentationStates.Adding
            };

            var videoModels = await this.powerPointParser.ParseFileAsync(presentationModel.Id, fileStream, cancellationToken);

            return presentationModel;
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "fileName: {FileName}", file.FileName);
            return null;
        }
    }

    #endregion
}