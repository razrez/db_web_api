using System;
using DB.Data;
using DB.Data.Repository;
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
    private readonly ISpotifyRepository _repository; //чтобы всёе краш лось отдельно добавлю, пока метода не вынес
    private readonly UserManager<UserInfo> _userManager;
    public UserContentController(ILogger<UserContentController> logger, SpotifyContext ctx, UserManager<UserInfo> userManager, ISpotifyRepository repository)
    {
        _logger = logger;
        _ctx = ctx;
        _userManager = userManager;
        _repository = repository;
    }

    [HttpGet]
    [Route("name/user/{userId}", Name="GetUserName")]
    public async Task<IActionResult> GetUserName(string userId)
    {
        var name = await _userManager.FindByIdAsync(userId);
        if (name != null)
        {
            return new JsonResult(new {Name = name.UserName});
        }
        return NotFound(new {Error = "Unexpected id"});
    }
    
    //5f34130c-2ed9-4c83-a600-e474e8f48bac
    [HttpGet]
    [Route("playlists/user/{userId}", Name="GetPlaylists")]
    public async Task<IActionResult> GetPlaylists(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new {Error = "Unexpected id"});
        }
        
        //свяжем для примера имеющиеся в бд песни с плейлистами, плейлисты с пользователем
        //один раз использовал - закоммитить можно
        await LikeAllSongs(user);

        var test = _repository.GetUsersPlaylists(userId).Result
            .Select(s => new
            {
                s.Id, s.UserId, s.Title, s.PlaylistType,
                Songs = s.Songs.Select(sk => new
                {
                    sk.Id, sk.UserId, sk.Name, sk.Source
                })
            });
        
        return new JsonResult(test);
    }

    private async Task LikeAllSongs(UserInfo user)
    {
        var songs = await _ctx.Songs.ToListAsync();
        var playlist = await _ctx.Playlists.FirstAsync();
        //like song
        foreach (var song in songs) playlist.Songs.Add(song);
        playlist.Users.Add(user); //нужно чтобы по дефолту при
                                  //создании пользователя у него был плейлист LikedSongs
                                  //а при создании плейлиста пользователем надо их связать через индекс LikedPlaylists
        if (!_ctx.Playlists.Contains(playlist))
        {
            _ctx.Playlists.Update(playlist);
            
            var res = await _ctx.SaveChangesAsync();
        }
    }
    private async Task Test(UserInfo user)
    {
        var songs = await _ctx.Songs.ToListAsync();
        var playlist = await _ctx.Playlists.FirstAsync();
        //like song
        foreach (var song in songs) _repository.LikeSong(user,song);
        playlist.Users.Add(user);
        if (!_ctx.Playlists.Contains(playlist))
        {
            _ctx.Playlists.Update(playlist);
            
            var res = await _ctx.SaveChangesAsync();
        }
    }
}
