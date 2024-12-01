using Microsoft.Extensions.Options;
using PowerPointService.Types;
using Xabe.FFmpeg;

namespace PowerPointService.Services;

public class FFMpegService : IFFMpegService
{
    #region Constructors

    public FFMpegService(IOptions<SettingOptions> options)
    {
        if (!string.IsNullOrEmpty(options.Value.FFMpegPath))
        {
            FFmpeg.SetExecutablesPath(options.Value.FFMpegPath);
        }
    }

    #endregion

    #region Public Methods

    public Task<IMediaInfo> GetMediaInfoAsync(string fileName, CancellationToken cancellationToken) =>
        FFmpeg.GetMediaInfo(fileName, cancellationToken);

    #endregion

    #region Private Methods

    #endregion
}