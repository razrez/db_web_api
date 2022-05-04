using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using DB.Attributes;
using DB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
public class HomePageController : ControllerBase
{
    private readonly SpotifyContext _context;

    public HomePageController(SpotifyContext context)
    {
        _context = context;
    }
    
    [HttpGet("/playlists/random"), AuthorizeWithJwt]
    public async Task<IActionResult> GetRandomPlaylists(int count)
    {
        var playlistsCount = _context.Playlists.Count();
        if (count > playlistsCount)
        {
            count = playlistsCount;
        }
        if(count == 0)
        {
            return BadRequest("Сan't get zero playlists");
        }
        var playlists = await _context.Playlists.OrderBy(r => Guid.NewGuid()).Take(count).ToListAsync();
        var jsonResult = JsonSerializer.Serialize(playlists);
        return Ok(jsonResult);
    }
}