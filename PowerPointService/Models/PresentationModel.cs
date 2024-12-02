using System.Text.Json.Serialization;
using PowerPointService.Types;

namespace PowerPointService.Models;

public record PresentationModel
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    [JsonIgnore]
    public string FullFileName { get; set; }
    
    public PresentationStates State { get; set; }
}