using DocumentFormat.OpenXml.Packaging;

namespace PowerPointService.Helpers;

public static class PowerPointHelper
{
    #region Public Methods

    public static string MapExtension(this MediaDataPart mediaDataPart) =>
        mediaDataPart.ContentType switch
        {
            "video/mp4" => ".mp4",
            "video/avi" => ".avi",
            "video/mov" => ".mov",
            "video/wmv" => ".wmv",
            _ => ".bin"
        };

    #endregion
}