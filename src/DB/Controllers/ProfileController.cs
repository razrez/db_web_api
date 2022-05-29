using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DB.Models.EnumTypes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;
using DB.Data;
using DB.Data.Repository;

namespace DB.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<UserInfo> _userManager;
        private readonly ISpotifyRepository _ctx;
        public ProfileController(UserManager<UserInfo> userManager, ISpotifyRepository ctx)
        {
            _userManager = userManager;
            _ctx = ctx;
        }

        [HttpGet("getProfile")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            var profile = await _ctx.GetProfile(userId);
            if (profile == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new DateOnlyConverter());

            return new JsonResult(profile, options);
        }
        
        [HttpPost("changeProfile/{userId},{username},{country},{birthday},{email}")]
        public async Task<IActionResult> ChangeProfile(string userId, string username, Country country, string birthday, string email)
        {
            if (!ModelState.IsValid) return BadRequest("not a valid model");

            var createRes = await _ctx.ChangeProfile(userId, username, country, birthday, email);
            
            return createRes ? Ok("changes accepted") : BadRequest(new {Error = "something went wrong"});
        }
        
        public static DateOnly Parse(string s)
        {
            var str = s.Split('.');

            int[] array = new int[3];
            array[0] = int.Parse(str[0]);
            array[1] = int.Parse(str[1]);
            array[2] = int.Parse(str[2]);

            return new DateOnly(array[0], array[1], array[2]);
        }

        [HttpPost("changePassword/{userId},{oldPassword},{newPassword}")]
        public async Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            if (!ModelState.IsValid) return BadRequest("not a valid model");

            var createRes = await _ctx.ChangePassword(userId, oldPassword, newPassword);
            
            return createRes ? Ok("password changed") : BadRequest(new {Error = "something went wrong"});
        }
        
        [HttpPost("changePremium/{userId},{premiumType}")]
        public async Task<IActionResult> ChangePremium(string userId, PremiumType premiumType)
        {
            if (!ModelState.IsValid) return BadRequest("not a valid model");

            var createRes = await _ctx.ChangePremium(userId, premiumType);
            
            return createRes ? Ok("changes accepted") : BadRequest(new {Error = "something went wrong"});
        }

}
}

