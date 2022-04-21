using System.Data.Common;
using DB.Attributes;
using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
public class TestSpotifyController : ControllerBase
{
    private readonly ILogger<TestSpotifyController> _logger;
    private readonly SpotifyContext _ctx;
    public TestSpotifyController(ILogger<TestSpotifyController> logger, SpotifyContext ctx)
    {
        _logger = logger;
        _ctx = ctx;
    }

    [HttpGet(Name = "GetProfiles")]
    public string Get()
    {
        //return context.Songs.ToList();
        var userInfo = new UserInfo
        {
            Email = "email@mail.ru",
            PasswordHash = "228пудриносик"
        };
        var profile = new Profile
        {
            Username = "user",
            Country = Country.Ukraine,
            UserType = UserType.User, 
            User = userInfo
        };
        
        _ctx.Profiles.Add(profile);
        _ctx.SaveChanges();
        return JsonConvert.SerializeObject( _ctx.Profiles.FirstOrDefault());
    }
}
