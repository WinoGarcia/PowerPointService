using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PowerPointService.Services;
using PowerPointService.Types;

namespace PowerPointService.Endpoints;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "slides")]
public class SlidesController : ControllerBase
{
    #region Private Fields

    private readonly ILogger<SlidesController> logger;
    private readonly IPowerPointService powerPointService;

    #endregion

    #region Constructors

    public SlidesController(ILogger<SlidesController> logger, IPowerPointService powerPointService)
    {
        this.logger = logger;
        this.powerPointService = powerPointService;
    }

    #endregion

    #region Endpoints

    [HttpPost("[action]", Name = "UploadFile")]
    [ProducesResponseType(typeof(PresentationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> UploadFileAsync(
        [Required] IFormFile file,
        CancellationToken cancellationToken)
    { 
        var presentationModel = await this.powerPointService.SaveFileAsync(file, cancellationToken);
        if (presentationModel is not null)
        {
            return this.Ok(this.powerPointService);
        }

        return this.BadRequest();
    }

    [HttpGet("[action]", Name = "GetVideos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetVideosAsync(
        [Required] Guid id,
        CancellationToken cancellationToken) => this.Ok();

    #endregion
}