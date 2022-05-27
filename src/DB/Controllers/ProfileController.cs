using System.Text.Json;
using DB.Models;
using Microsoft.AspNetCore.Mvc;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DB.Data;
using DB.Infrastructure;

namespace DB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly SpotifyContext _ctx;
        private readonly UserManager<UserInfo> _userManager;
        public ProfileController(SpotifyContext ctx, UserManager<UserInfo> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }


        [HttpGet]
        [Route("user/getProfile/{userId}", Name = "GetProfile")]
        public async Task<IActionResult> Get(string userId)
        {
            var profile = await _ctx.Profiles
                .Include(u => u.User)
                .AsSplitQuery()
                .Where(x => x.UserId == userId)
                .Select(s => new
                {
                    s.Username,
                    s.UserType,
                    s.Birthday,
                    s.Country,
                    email = s.User!.Email,
                })
                .ToListAsync();
            
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new DateOnlyConverter());
            
            return new JsonResult(profile, options);
        }

        [HttpPost]
        [Route("user/editProfile/{userId},{username},{country}, {birthday}, {email}", Name = "EditProfile")]
        public async Task<IActionResult> ChangeProfile(string? userId, string? username, Country country, string? birthday, string? email)
        {
            if (userId == null || username == null || country == null || birthday == null || email == null)
            {
                return BadRequest("Fields cannot be empty");
            }
            
            var date = Parse(birthday);

            var user = await _userManager.FindByIdAsync(userId);
            user.Email = email;
            user.NormalizedEmail = email.ToUpper();
            user.NormalizedUserName = username.ToUpper();

            var profile = _ctx.Profiles.FirstOrDefault(x => x.UserId == userId);

            if (profile != null)
            {
                profile.Username = username;
                profile.Country = country;
                profile.Birthday = date;

                await _userManager.UpdateAsync(user);

                _ctx.Profiles.Update(profile);
            }
            else
            {
                return NotFound("Failed to find user profile");
            }

            await _ctx.SaveChangesAsync();

            var profileJson = await _ctx.Profiles
                .Include(x => x.User)
                .AsSplitQuery()
                .Where(x => x.UserId == userId)
                .Select(s => new
                {
                    s.Username,
                    s.UserType,
                    s.Birthday,
                    s.Country,
                    s.User.Email
                })
                .ToListAsync();
            
            return Ok("Changes done");
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

        [HttpPost]
        [Route("user/changePassword/{userId},{oldPassword},{newPassword}", Name = "ChangePassword")]
        public async Task<IActionResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (oldPassword == newPassword)
            {
                return BadRequest("New password must be different from the old");
            }

            if (user == null)
            {
                return NotFound("User not found");
            }

            var res = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (res.Succeeded)
            {
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            else
                return BadRequest("Error change password");
        }

        [HttpPost]
        [Route("user/changePremium/{userId},{premiumType}")]
        public async Task<IActionResult> ChangePremium(string userId, PremiumType premiumType)
        {
            var premium = _ctx.Premia.FirstOrDefaultAsync(x => x.UserId == userId).Result;
            if (premium == null)
            {
                throw new Exception("User not found");
            }
            premium.PremiumType = premiumType;
            DateTime date = DateTime.Now;
            premium.StartAt = date;
            premium.EndAt = date.AddMonths(1);
            
            _ctx.Premia.Update(premium);

            await _ctx.SaveChangesAsync();
            
            return Ok("Change done");
        }

    }
}

