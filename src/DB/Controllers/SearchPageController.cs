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
        return new JsonResult(result);
    }
    
}