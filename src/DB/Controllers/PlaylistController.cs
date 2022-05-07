using DB.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class PlaylistController : ControllerBase
{
    private readonly ISpotifyRepository _ctx;

    public PlaylistController(ISpotifyRepository ctx)
    {
        _ctx = ctx;
    }

    [HttpDelete("{playlistId}")]
    public async Task<IActionResult> DeletePlaylist(int playlistId)
    {
        var res = await _ctx.DeletePlaylist(playlistId);
        return res ? Ok() : NotFound();
    }
}