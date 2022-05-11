using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using DB.Attributes;
using DB.Data;
using DB.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
public class HomePageController : ControllerBase
{
    private readonly ISpotifyRepository _context;

    public HomePageController(ISpotifyRepository context)
    {
        _context = context;
    }
    
    [AuthorizeWithJwt]
    [HttpGet("/playlists/random")]
    public async Task<IActionResult> GetRandomPlaylists(int count)
    {
        var playlistsCount = _context.GetPlaylistsCount();
        if (count > playlistsCount)
        {
            count = playlistsCount;
        }
        if(count == 0)
        {
            return BadRequest("Сan't get zero playlists");
        }

        var playlists = await _context.GetRandomPlaylists(count);
        var jsonResult = JsonSerializer.Serialize(playlists);
        return Ok(jsonResult);
    }
}