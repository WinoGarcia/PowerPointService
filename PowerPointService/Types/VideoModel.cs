namespace PowerPointService.Types;

public record VideoModel
{
    public Guid PresentationId { get; init; }

    public int SlideId { get; init; }

    public Guid Id { get; init; }

    public string Name { get; set; }

    public string FullFileName { get; set; }

    public TimeSpan Duration { get; set; }
}