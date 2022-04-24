using System.Data.Common;
using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class UserContentController : ControllerBase
{
    private readonly ILogger<UserContentController> _logger;
    private readonly SpotifyContext _ctx;
    private readonly UserManager<UserInfo> _userManager;
    public UserContentController(ILogger<UserContentController> logger, SpotifyContext ctx, UserManager<UserInfo> userManager)
    {
        _logger = logger;
        _ctx = ctx;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("name/user/{userId}", Name="GetUserName")]
    public JsonResult GetUserName(string userId)
    {
        var res = _ctx.Users
            .AsSplitQuery()
            .Where(us => us.Id == userId)
            .Select(name => name.UserName).First();
        return new JsonResult(res);
    }
    
    [HttpGet]
    [Route("playlists/user/{userId}", Name="GetPlaylists")]
    public JsonResult GetPlaylists(string userId)
    {
        var user = _userManager.FindByIdAsync(userId).Result;
        
        var song = new Song
        {
            Name = "song1",
            Source = "source1",
            User = user
        };
        var song2 = new Song
        {
            Name = "song2",
            Source = "source2",
            User = user
        };
        
        var playlist = new Playlist
        {
            Title = "Playlist",
            PlaylistType = PlaylistType.User,
            User = user,
        };
        playlist.Songs.Add(song);
        playlist.Songs.Add(song2);

        _ctx.Playlists.Add(playlist);
        _ctx.SaveChanges();
        return new JsonResult(user);
    }
}
