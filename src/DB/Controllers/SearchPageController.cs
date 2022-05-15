using DB.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DB.Controllers;

[ApiController]
[Route("api/search")]
public class SearchPageController : ControllerBase
{
    private readonly ISpotifyRepository _context;

    public SearchPageController(ISpotifyRepository context)
    {
        _context = context;
    }

    [HttpPost("playlists")]
    public async Task<IActionResult> SearchPlaylist(string input)
    {
        var result = await _context.SearchPlaylists(input);
        if (result.Count == 0)
            return NotFound();
        return new JsonResult(result);
    }
    
    [HttpPost("songs")]
    public async Task<IActionResult> SearchSongs(string input)
    {
        var result = await _context.SearchSongs(input);
        if (result.Count == 0)
            return NotFound();
        return new JsonResult(result);
    }
    
    [HttpPost("users")]
    public async Task<IActionResult> SearchUsers(string input)
    {
        var result = await _context.SearchProfile(input, false);
        if (result.Count == 0)
            return NotFound();
        return new JsonResult(result);
    }
    
    [HttpPost("artists")]
    public async Task<IActionResult> SearchArtists(string input)
    {
        var result = await _context.SearchProfile(input, true);
        if (result.Count == 0)
            return NotFound();
        return new JsonResult(result);
    }
    
}