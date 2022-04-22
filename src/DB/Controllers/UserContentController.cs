using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserContentController : ControllerBase
{
    private readonly UserManager<UserInfo> _userManager;
    private readonly SpotifyContext _ctx;
    // GET

    public UserContentController(SpotifyContext ctx, UserManager<UserInfo> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    public Task<string> GetName(string id)
    {
        var resultUserName = _userManager
            .FindByIdAsync(id)
            .Result.UserName.FirstOrDefault();

        return Task.FromResult(resultUserName.ToString());
    }
    
    [HttpGet("{id}")]
    public IEnumerable<Playlist> GetPlaylists(string id)
    {
        //
        var playlists = _ctx.Playlists
            .Where(s => s.UserId == id)
            .AsSplitQuery().ToList();
        return playlists;
    }
    
}