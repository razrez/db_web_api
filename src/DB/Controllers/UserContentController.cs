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
    
    [HttpGet]
    [Route("playlists/user/", Name="GetPlaylists")]
    public async Task<JsonResult> GetPlaylists()
    {
        /*var user2 = new UserInfo()
        {
            Id = "5f34130c-2ed9-4c83-a600-e474e8f42bac",
            UserName = "user02@gamil.com",
            Email = "user02@gamil.com",
            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
            EmailConfirmed = true
        };
        
        //await _userManager.CreateAsync(user2);
        
        //step2
        var playlist = new Playlist()
        {
            UserId = user2.Id,
            Title = "playlist3",
            PlaylistType = PlaylistType.User,
            ImgSrc = "src2",
            Verified = true,
        };
        
        var song1 = new Song()
        {
            Name = "song1",
            Source = "src1",
            User = user2
        };
        song1.Playlists.Add(playlist);

        user2.Playlists.Add(playlist);
        await _ctx.AddAsync(user2);
        await _ctx.Songs.AddAsync(song1);
        
        await _ctx.SaveChangesAsync();*/
        
        /*var user = await _userManager.FindByIdAsync("5f34130c-2ed9-4c83-a600-e474e8f48bac");
        var pl = await _ctx.Playlists.FirstAsync();
        pl.Users.Add(user);
        _ctx.Playlists.Update(pl);
        await _ctx.SaveChangesAsync();*/
        var userId = "5f34130c-2ed9-4c83-a600-e474e8f42bac";
        var pl = _ctx.Playlists
            .Include(x => x.Songs)
            .Include(x => x.Users)
            .AsSplitQuery()
            .Where(k => k.UserId == userId)
            .ToList();
        return new JsonResult(pl);
    }
}
