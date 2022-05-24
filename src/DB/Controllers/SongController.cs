using DB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DB.Data.Repository;

namespace DB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]

    public class SongController : ControllerBase
    {
        private readonly UserManager<UserInfo> _userManager;
        private readonly ISpotifyRepository _ctx;
        
        public SongController(ISpotifyRepository ctx, UserManager<UserInfo> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("getSong/{songId}")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await _ctx.GetSong(songId);
            if (song == null) return NotFound(new {Error = "not found"});

            var result = new JsonResult(new
            {
                song.Id, song.UserId, song.Name, song.Source
            });
            return result;
        }

        [HttpPost("addSongToPlaylist")]
        public async Task<IActionResult> AddSongToPlaylist(int songId, int playlistId)
        {
            if (!ModelState.IsValid) return BadRequest("not a valid model");

            var createRes = await _ctx.AddSongToPlaylist(songId, playlistId);
            
            return createRes ? Ok() : BadRequest(new {Error = "something went wrong"});
        }
    }
    
    
}