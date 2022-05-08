using DB.Data.Repository;
using DB.Models;
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

    [HttpDelete("delete/{playlistId}")]
    public async Task<IActionResult> DeletePlaylist(int playlistId)
    {
        var res = await _ctx.DeletePlaylist(playlistId);
        return res ? Ok() : NotFound("playlist not found");
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditPlaylist(Playlist newPlaylist)
    {
        if (!ModelState.IsValid) return BadRequest("not a valid model");
        var editRes = await _ctx.EditPlaylist(newPlaylist);
        return editRes ? Ok() : BadRequest(new {Error = "something went wrong"});
    }
}