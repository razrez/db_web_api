using System;
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
    public async Task<JsonResult> GetUserName(string userId)
    {
        var name = await _userManager.FindByIdAsync(userId);
        return new JsonResult(name.UserName);
    }
    
    //5f34130c-2ed9-4c83-a600-e474e8f48bac
    [HttpGet]
    [Route("playlists/user/{userId}", Name="GetPlaylists")]
    public async Task<JsonResult> GetPlaylists(string userId)
    {
        //свяжем для примера имеющиеся в бд песни с плейлистами, плейлисты с пользователем
        //один раз использовал - закоммитить можно
        await LikeAllSongs(userId);

        var usersPlaylists = await _ctx.Playlists
            .Include(x => x.Songs)
            .Include(x => x.Users)
            .AsSplitQuery()
            .Where(k => k.UserId == userId)
            .Select(s => new
            {
                s.Id, s.UserId, s.Title, s.PlaylistType,
                Songs = s.Songs.Select(sk => new
                {
                    sk.Id, sk.UserId, sk.Name, sk.Source
                })
            })
            .ToListAsync();
        return new JsonResult(usersPlaylists);
    }

    private async Task LikeAllSongs(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var songs = await _ctx.Songs.ToListAsync();
        var playlist = await _ctx.Playlists.FirstAsync();
        foreach (var song in songs) playlist.Songs.Add(song);
        playlist.Users.Add(user);
        _ctx.Playlists.Update(playlist);
        
        await _ctx.SaveChangesAsync();
    }
}
