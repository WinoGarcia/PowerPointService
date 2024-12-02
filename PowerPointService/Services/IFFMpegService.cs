using Xabe.FFmpeg;

namespace PowerPointService.Services;

public interface IFFMpegService
{
    Task<IMediaInfo> GetMediaInfoAsync(string fileName, CancellationToken cancellationToken);
}