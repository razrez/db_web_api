using System.Threading.Tasks;
using DB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DB.Data.Repository;

namespace DB.Controllers
{
    [ApiController]
    [Route("api/song")]

    public class SongController : ControllerBase
    {
        private readonly ISpotifyRepository _ctx;
        
        public SongController(ISpotifyRepository ctx)
        {
            _ctx = ctx;
        }

        [HttpGet("getSong")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await _ctx.GetSong(songId);
            if (song == null) return NotFound(new {Error = "song not found"});

            return new JsonResult(song);
        }

        [HttpPost("addSongToPlaylist")]
        public async Task<IActionResult> AddSongToPlaylist([FromForm]int songId, [FromForm]int playlistId)
        {
            if (!ModelState.IsValid) return BadRequest("not a valid model");

            var createRes = await _ctx.AddSongToPlaylist(songId, playlistId);
            
            return createRes ? Ok("song added to playlist") : BadRequest(new {Error = "the song/playlist was not found or already added"});
        }
    }
    
    
}