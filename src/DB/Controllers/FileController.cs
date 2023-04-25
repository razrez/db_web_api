using DB.Data.Repository;
using DB.Models;
using Microsoft.AspNetCore.Mvc;
using Path = System.IO.Path;

namespace DB.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly ISpotifyRepository _context;
    private static string _directoryPath = null!;

    public FileController(IWebHostEnvironment env, ISpotifyRepository context)
    {
        _context = context;
        _directoryPath = Path.Combine(env.ContentRootPath, "..\\..\\files\\");
    }

    [HttpGet("song")]
    public IActionResult GetSongFile(int songId)
    {
        /*var songSource = _context.GetSong(songId).Result.Source;
        var songPath = Path.Combine(_directoryPath, $"songs\\{songSource}");
        var song = System.IO.File.ReadAllBytes(songPath);
        return File(song, "audio/mpeg", songSource);*/
        
        var song = _context.GetSong(songId).Result;
        if(song == null)
            return BadRequest("Wrong song ID");
        var songSrc = song.Source;
        var songPath = Path.Combine(_directoryPath, $"songs\\{songSrc}");
        var img = System.IO.File.ReadAllBytes(songPath);
        Response.Headers.Add("Accept-Ranges", "bytes");
        return File(img, "audio/mpeg", songSrc);
    }
    
    [HttpGet("picture/user")]
    public IActionResult GetUserPicture(string userId)
    {
        var profile = _context.GetProfile(userId).Result;
        if(profile == null)
            return BadRequest("Wrong user ID");
        var profileImgSrc = profile.ProfileImg ?? "user_test.jpg";
        var imgPath = Path.Combine(_directoryPath, $"pictures\\users\\{profileImgSrc}");
        var img = System.IO.File.ReadAllBytes(imgPath);
        return File(img, "image/jpeg", profileImgSrc);
    }
    
    [HttpGet("picture/playlist")]
    public IActionResult GetPlaylistPicture(int playlistId)
    {
        var playlist = _context.GetPlaylistInfo(playlistId).Result;
        if (playlist == null)
            return BadRequest("Wrong playlist ID");
        var playlistImgSrc = playlist.ImgSrc ?? "playlist_test.jpg";
        var imgPath = Path.Combine(_directoryPath, $"pictures\\playlists\\{playlistImgSrc}");
        var img = System.IO.File.ReadAllBytes(imgPath);
        return File(img, "image/jpeg", playlistImgSrc);
    }
}