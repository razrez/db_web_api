using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
public class TestSpotifyController : ControllerBase
{
    private readonly ILogger<TestSpotifyController> _logger;

    public TestSpotifyController(ILogger<TestSpotifyController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetSong")]
    public IEnumerable<Profile> Get()
    {
        using var context = new SpotifyContext();
        //return context.Songs.ToList();
        var profile = new Profile
        {
            Username = "user",
            Country = Country.Russia,
            UserType = UserType.User
        };
        /*context.Profiles.Add(profile);
        context.SaveChanges();*/
        return context.Profiles.ToList();
    }
}
