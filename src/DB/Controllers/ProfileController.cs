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
using DB.Infrastructure;

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
            var profile = _ctx.GetProfile(userId).Result;
            if (profile == null) return NotFound("user not found");
            
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new DateOnlyConverter());

            return new JsonResult(profile, options);
        }
        
        [HttpPost("changeProfile")]
        public async Task<IActionResult> ChangeProfile([FromForm]string userId, [FromForm]string username, [FromForm]Country country, [FromForm]string birthday, [FromForm]string email)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null) return NotFound("User not found");

            var createRes = await _ctx.ChangeProfile(userId, username, country, birthday, email);
            
            return createRes ? Ok("changes accepted") : NotFound(new {Error = "not found"});
        }
        
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromForm]string userId, [FromForm]string oldPassword, [FromForm]string newPassword)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null) return NotFound("User not found");
            var createRes = await _ctx.ChangePassword(user, oldPassword, newPassword);
            
            return createRes ? Ok("password changed") : BadRequest(new {Error = "password wrong"});
        }
        
        [HttpPost("changePremium")]
        public async Task<IActionResult> ChangePremium([FromForm]string userId, [FromForm]PremiumType premiumType)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null) return NotFound("User not found");

            var createRes = await _ctx.ChangePremium(userId, premiumType);
            
            return createRes ? Ok("changes accepted") : BadRequest(new {Error = "you already have this premium"});
        }

}
}

