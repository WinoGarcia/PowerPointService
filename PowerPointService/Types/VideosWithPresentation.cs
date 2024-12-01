using PowerPointService.Models;

namespace PowerPointService.Types;

public record VideosWithPresentation()
{
    public string PresentationId { get; init; }

    public int SlideId { get; init; }

    public string Id { get; init; }

    public string Name { get; set; }

    public string FullFileName { get; set; }

    public string Duration { get; set; }

    public string PresentationName { get; init; }

    public string PresentationFullFileName { get; set; }

    public PresentationStates PresentationState { get; set; }

    public PresentationModel MapToPresentation() =>
        new()
        {
            Id = Guid.Parse(this.PresentationId),
            Name = this.PresentationName,
            FullFileName = this.PresentationFullFileName,
            State = this.PresentationState
        };

    public VideoModel MapToVideo() =>
        new()
        {
            PresentationId = Guid.Parse(this.PresentationId),
            SlideId = this.SlideId,
            Id = Guid.Parse(this.Id),
            Name = this.Name,
            FullFileName = this.FullFileName,
            Duration = TimeSpan.Parse(this.Duration)
        };
}