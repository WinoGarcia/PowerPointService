namespace PowerPointService.Types;

public record PresentationModel
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string FullFileName { get; set; }

    public PresentationStates State { get; set; }
}