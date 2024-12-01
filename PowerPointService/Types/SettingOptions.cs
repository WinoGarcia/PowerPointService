namespace PowerPointService.Types;

public sealed class SettingOptions
{
    public const string Settings = nameof(Settings);

    public string FFMpegPath { get; set; }

    public string PathBase { get; set; }

    public string VideoPath { get; set; }
}