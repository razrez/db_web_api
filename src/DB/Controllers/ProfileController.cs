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
            var profile = await _ctx.GetProfile(userId);
            if (profile == null) return NotFound("user not found");
            
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new DateOnlyConverter());

            return new JsonResult(profile, options);
        }
        
        [HttpPut("changeProfile")]
        public async Task<IActionResult> ChangeProfile([FromForm]string userId, [FromForm]string? username, [FromForm]Country? country, [FromForm]string? birthday, [FromForm]string? email)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");
    
            var createRes = await _ctx.ChangeProfile(userId, username, country, birthday, email);
                
            return createRes ? Ok("changes accepted") : NotFound(new {Error = "not found"});
        }
        
        [HttpPut("change")]
        [Produces("application/json")]
        public async Task<IActionResult> ChangeProfile([FromBody] ChangeProfileForm changeProfileForm)
        {
            var user = await _userManager.FindByIdAsync(changeProfileForm.userId);
            if (user == null) return NotFound("User not found");
    
            var createRes = await _ctx.ChangeProfile(changeProfileForm.userId, changeProfileForm.username, (Country?)int.Parse(changeProfileForm.country!), changeProfileForm.birthday, changeProfileForm.email);
                
            return createRes ? Ok("changes accepted") : NotFound(new {Error = "not found"});
        }
        
        #region just for JSON mapping
        public class ChangePasswordForm
        {
            public string userId { get; set; }
            public string oldPassword { get; set; }
            public string newPassword { get; set; }
        }
        
        public class ChangeProfileForm
        {
            public string userId { get; set; }
            public string? username { get; set; }
            public string? country { get; set; }
            public string? birthday { get; set; }
            public string? email { get; set; }
        }
        #endregion
        
        [HttpPost("password/change")]
        [Produces("application/json")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordForm changePasswordForm)
        {
            var user = await _userManager.FindByIdAsync(changePasswordForm.userId);
            if (user == null) return NotFound("User not found");
            var createRes = await _ctx.ChangePassword(user, changePasswordForm.oldPassword, changePasswordForm.newPassword);
            
            return createRes ? Ok("password changed") : BadRequest(new {Error = "password wrong"});
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
        public async Task<IActionResult> ChangePremium([FromForm]string userId, [FromForm]int premiumId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null) return NotFound("User not found");

            var createRes = await _ctx.ChangePremium(userId, premiumId);
            
            return createRes ? Ok("changes accepted") : BadRequest(new {Error = "you already have this premium"});
        }

        [HttpGet("user_premium/{userId}")]
        public async Task<IActionResult> GetUserPremium(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                return BadRequest("User not fount");
            var premium = await _ctx.GetUserPremium(userId);
            if (premium == null)
                return BadRequest("User's premium not found");
            return Ok(premium);
        }

        [HttpGet("premiums/{userId}")]
        public async Task<IActionResult> GetAvailablePremiums(string userId)
        {
            var premiums = await _ctx.GetAvailablePremiums(userId);
            if (premiums == null)
                return BadRequest("wrong user id");
            return Ok(premiums);
        }

        [HttpGet("premium/{premiumId}")]
        public async Task<IActionResult> GetPremium(int premiumId)
        {
            var premium = await _ctx.GetPremium(premiumId);
            if (premium == null)
                return BadRequest("wrong premium id");
            return Ok(premium);
        }
    }
}

