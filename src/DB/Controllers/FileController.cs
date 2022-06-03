using DB.Data.Repository;
using DB.Models;
using Microsoft.AspNetCore.Mvc;

namespace DB.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly ISpotifyRepository _context;
    private readonly IWebHostEnvironment _env;

    public FileController(IWebHostEnvironment env, ISpotifyRepository context)
    {
        _env = env;
        _context = context;
    }

    [HttpGet("/song")]
    public IActionResult GetSongFile(int songId)
    {
        var root = _env.ContentRootPath;
        var directory = Path.Combine(root, "..\\..\\Files\\Songs\\");
        var songSource = _context.GetSong(songId).Result.Source;
        var songPath = Path.Combine(directory, songSource);
        var song = System.IO.File.ReadAllBytes(songPath);
        return Ok(File(song, "audio/mpeg"));
    }
}