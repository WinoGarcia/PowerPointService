using PowerPointService.Models;

namespace PowerPointService.Types;

public record VideosDto()
{
    public PresentationModel Presentation { get; set; }

    public List<GroupedBySlide> GroupedVideos { get; set; } = [];
}

public record GroupedBySlide
{
    public int SlideId { get; set; }

    public List<VideoModel> Videos { get; set; }
}