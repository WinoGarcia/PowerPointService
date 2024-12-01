using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace PowerPointService.Endpoints;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "slides")]
public class SlidesController : ControllerBase
{
    #region Endpoints

    [HttpPost("[action]", Name = "UploadFile")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> UploadFileAsync(
        [Required] IFormFile file,
        CancellationToken cancellationToken) =>
        this.Ok(Guid.NewGuid());

    [HttpGet("[action]", Name = "GetVideos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetVideosAsync(
        [Required] Guid id,
        CancellationToken cancellationToken) =>
        this.Ok();

    #endregion
}