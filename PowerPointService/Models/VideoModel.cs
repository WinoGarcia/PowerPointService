using System.Text.Json.Serialization;

namespace PowerPointService.Models;

public record VideoModel
{
    [JsonIgnore]
    public Guid PresentationId { get; init; }

    [JsonIgnore]
    public int SlideId { get; init; }

    public Guid Id { get; init; }

    public string Name { get; set; }

    [JsonIgnore]
    public string FullFileName { get; set; }

    public TimeSpan Duration { get; set; }
}