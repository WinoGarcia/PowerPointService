using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PowerPointService.Models;
using PowerPointService.Services;
using PowerPointService.Types;

namespace PowerPointService.Endpoints;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "slides")]
public class SlidesController : ControllerBase
{
    #region Private Fields

    private readonly IPowerPointService powerPointService;

    #endregion

    #region Constructors

    public SlidesController(IPowerPointService powerPointService)
    {
        this.powerPointService = powerPointService;
    }

    #endregion

    #region Endpoints

    [HttpPost("[action]", Name = "UploadPresentation")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PresentationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> UploadPresentationAsync(
        [Required] IFormFile file,
        CancellationToken cancellationToken)
    {
        var presentationModel = await this.powerPointService.SaveFileAsync(file, cancellationToken);
        if (presentationModel is not null)
        {
            return this.Ok(presentationModel);
        }

        return this.BadRequest();
    }

    [HttpGet("[action]", Name = "GetVideos")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(VideosDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VideosDto>> GetVideosAsync(
        [Required] Guid presentationId,
        CancellationToken cancellationToken)
    {
        var vodeoModelds = await this.powerPointService.GetVideosAsync(presentationId, cancellationToken);
        if (vodeoModelds is not null)
        {
            return this.Ok(vodeoModelds);
        }

        return this.NotFound();
    }

    [HttpGet("[action]", Name = "DownloadVideo")]
    [Produces(MediaTypeNames.Application.Octet)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DownloadVideoAsync(
        [Required] Guid videoId,
        CancellationToken cancellationToken)
    {
        var videoContent = await this.powerPointService.GetVideoContentAsync(videoId, cancellationToken);
        if (videoContent is not null)
        {
            var file = await System.IO.File.ReadAllBytesAsync(videoContent.FullFileName, cancellationToken);
            return this.File(file, MediaTypeNames.Application.Octet, videoContent.Name);
        }

        return this.NotFound();
    }

    #endregion
}