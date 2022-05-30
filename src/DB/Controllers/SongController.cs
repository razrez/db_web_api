using DB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DB.Data.Repository;

namespace DB.Controllers
{
    [ApiController]
    [Route("api/song")]

    public class SongController : ControllerBase
    {
        private readonly UserManager<UserInfo> _userManager;
        private readonly ISpotifyRepository _ctx;
        
        public SongController(ISpotifyRepository ctx, UserManager<UserInfo> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpGet("getSong")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await _ctx.GetSong(songId);
            if (song.Name == "") return NotFound();

            return new JsonResult(song);
        }

        [HttpPost("addSongToPlaylist")]
        public async Task<IActionResult> AddSongToPlaylist(int songId, int playlistId)
        {
            if (!ModelState.IsValid) return BadRequest("not a valid model");

            var createRes = await _ctx.AddSongToPlaylist(songId, playlistId);
            
            return createRes ? Ok("song added to playlist") : BadRequest(new {Error = "the song was not added or already added"});
        }
    }
    
    
}