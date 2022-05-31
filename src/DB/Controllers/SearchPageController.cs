using System.Text.Json;
using System.Threading.Tasks;
using DB.Data.Repository;
using DB.Infrastructure;
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

    [HttpGet("playlists")]
    public async Task<IActionResult> SearchPlaylist(string input)
    {
        var result = await _context.SearchPlaylists(input);
        if (result.Count == 0)
            return NotFound();
        return new JsonResult(result);
    }
    
    [HttpGet("songs")]
    public async Task<IActionResult> SearchSongs(string input)
    {
        var result = await _context.SearchSongs(input);
        if (result.Count == 0)
            return NotFound();
        return new JsonResult(result);
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers(string input)
    {
        var result = await _context.SearchProfile(input, false);
        if (result.Count == 0)
            return NotFound();
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new DateOnlyConverter());
        return new JsonResult(result, options);
    }
    
    [HttpGet("artists")]
    public async Task<IActionResult> SearchArtists(string input)
    {
        var result = await _context.SearchProfile(input, true);
        if (result.Count == 0)
            return NotFound();
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new DateOnlyConverter());
        return new JsonResult(result, options);
    }
}