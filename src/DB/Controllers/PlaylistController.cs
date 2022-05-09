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
        return res ? Ok() : NotFound(new {Errpr = "playlist not found"});
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditPlaylist(Playlist newPlaylist)
    {
        if (!ModelState.IsValid) return BadRequest("not a valid model");
        
        var editRes = await _ctx.EditPlaylist(newPlaylist);
        
        return editRes ? Ok() : BadRequest(new {Error = "something went wrong"});
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePlaylist(Playlist newPlaylist)
    {
        if (!ModelState.IsValid) return BadRequest("not a valid model");
        
        var createRes = await _ctx.CreatePlaylist(newPlaylist);
        
        return createRes ? Ok() : BadRequest(new {Error = "something went wrong"});
    }

    [HttpGet("getInfo/{playlistId}")]
    public async Task<IActionResult> GetPlaylistInfo(int playlistId)
    {
        var playlist = await _ctx.GetPlaylistInfo(playlistId);
        if (playlist == null) return NotFound(new {Error = "playlist not found"});
        
        var result = new JsonResult(new
        {
            playlist.Id, playlist.UserId, playlist.Title, playlist.PlaylistType,
            Songs = playlist.Songs.Select(sk => new
            {
                sk.Id, sk.UserId, sk.Name, sk.Source
            })
        });
        
        return result;
    }

    [HttpPost("like")]
    public async Task<IActionResult> LikePlaylist(int playlistId, string userId)
    {
        var res = await _ctx.LikePlaylist(playlistId, userId);
        return res ? Ok() : BadRequest(new {Error = "something went wrong"});
    }
}