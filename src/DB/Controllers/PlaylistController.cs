using DB.Data.Repository;
using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Mvc;

namespace DB.Controllers;

[ApiController]
[Route("api/playlist")]
[Produces("application/json")]
public class PlaylistController : ControllerBase
{
    private readonly ISpotifyRepository _ctx;

    public PlaylistController(ISpotifyRepository ctx)
    {
        _ctx = ctx;
    }

    /// <summary>
    /// Deletes a playlist by given ID.
    /// </summary>
    /// <remarks>
    /// Предполагается, что мы достаем из принципала или из джвт токена userId(откуда там достается)
    /// и только потом удаляем плейлист. Это нужно, чтобы юзеры не могли удалятьь чужие плейлисты :)
    /// </remarks>
    /// <param name="playlistId"></param>
    /// <param name="userId"></param>
    /// <response code="200">If request is succeed.</response>
    /// <response code="404">If playlist with preferable ID doesn't exist.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlaylist(int playlistId, string userId)
    {
        var res = await _ctx.DeletePlaylist(playlistId, userId);
        return res ? Ok() : NotFound(new {Error = "not found"});
    }
    
    
    /// <summary>
    /// Edit a Playlist.
    /// </summary>
    /// <param name="newPlaylist"></param>
    /// <response code="200">If request is succeed.</response>
    /// <response code="400">Not valid request.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditPlaylist(Playlist newPlaylist)
    {
        if (!ModelState.IsValid) return BadRequest("not a valid model");
        
        var editRes = await _ctx.EditPlaylist(newPlaylist);
        
        return editRes ? Ok() : BadRequest(new {Error = "something went wrong"});
    }
    
    
    /// <summary>
    /// Creates a Playlist.
    /// </summary>
    /// <remarks>
    /// Minimal request JSON:
    ///
    ///     {
    ///        "title": "SuperTitle", 
    ///        "userId": "5f34130c-2ed9-4c83-a600-e474e8f48bac",
    ///        "playlistType": 3,
    ///        "genreType": 4,
    ///        "verified": true
    ///     }
    ///
    /// </remarks>
    /// <param name="newPlaylist"></param>
    /// <response code="200">If request is succeed.</response>
    /// <response code="400">Not valid request.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlaylist(Playlist newPlaylist)
    {
        if (!ModelState.IsValid) return BadRequest("not a valid model");
        
        var createRes = await _ctx.CreatePlaylist(newPlaylist);
        
        return createRes ? Ok() : BadRequest(new {Error = "something went wrong"});
    }

    
    /// <summary>
    /// Gets a playlist with full information.
    /// </summary>
    /// <param name="playlistId"></param>
    /// <remarks>
    /// Sample response:
    ///
    ///     {
    ///        "id": 2,
    ///        "userId": "5f34130c-2ed9-4c83-a600-e474e8f48bac",
    ///        "title": "simple playlist",
    ///        "playlistType": 3,
    ///        "genreType": 3,
    ///        "songs": []
    ///     }
    ///
    /// </remarks>
    /// <response code="200">If request is succeed.</response>
    /// <response code="404">If playlist with preferable ID doesn't exist.</response>
    [HttpGet("{playlistId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlaylistInfo(int playlistId)
    {
        var playlist = await _ctx.GetPlaylistInfo(playlistId);
        if (playlist == null) return NotFound(new {Error = "not found"});
        
        var result = new JsonResult(new
        {
            playlist.Id, playlist.UserId, playlist.Title,
            playlist.PlaylistType, playlist.GenreType,
            Songs = playlist.Songs.Select(sk => new
            {
                sk.Id, sk.UserId, sk.Name, sk.Source
            })
        });
        
        return result;
    }
    

    /// <summary>
    /// Likes a playlist.
    /// </summary>
    /// <param name="playlistId"></param>
    /// <param name="userId"></param>
    /// <response code="200">If request is succeed.</response>
    /// <response code="400">If playlist or user with preferable IDs don't exist.</response>
    [HttpPost("like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LikePlaylist(int playlistId, string userId)
    {
        var res = await _ctx.LikePlaylist(playlistId, userId);
        return res ? Ok() : BadRequest(new {Error = "something went wrong"});
    }
    

    /// <summary>
    /// Gets all playlists which were liked by user.
    /// </summary>
    /// <remarks>
    /// Sample response:
    ///     
    ///     [
    ///      {
    ///          "id": 1,
    ///          "userId": "5f34130c-2ed9-4c83-a600-e474e8f48bac",
    ///          "title": "LikedSongs",
    ///          "playlistType": 4,
    ///          "genreType": 4,
    ///          "songs": []
    ///      },
    ///      {
    ///          "id": 3,
    ///          "userId": "5f34130c-2ed9-4c83-a600-e474e8f48bac",
    ///          "title": "string",
    ///          "playlistType": 3,
    ///          "genreType": 0,
    ///          "songs": []
    ///      }
    ///     ]
    ///
    /// </remarks>
    /// <param name="userId"></param>
    /// <response code="200">If request is succeed.</response>
    /// <response code="404">If playlist with preferable ID doesn't exist.</response>
    [HttpGet("library/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserLibrary(string userId)
    {
        var userLibrary = await _ctx.GetUserLibrary(userId);
        if (userLibrary != null)
        {
            var result = userLibrary
                .Select(playlist => new
                {
                    playlist.Id, playlist.UserId, 
                    playlist.Title, playlist.PlaylistType, playlist.GenreType,
                    Songs = playlist.Songs.Select(sk => new
                    {
                        sk.Id, sk.UserId, sk.Name, sk.Source
                    })
                }); //offset
            
            return new JsonResult(result);
        }
        
        return NotFound(new {Error = "not found"});
    }


    /// <summary>
    /// Gets a random playlists by preferred GENRE TYPE.
    /// </summary>
    /// <param name="genreType"></param>
    /// <remarks>
    /// Sample response:
    ///     
    ///     [
    ///      {
    ///          "id": 1,
    ///          "userId": "5f34130c-2ed9-4c83-a600-e474e8f48bac",
    ///          "title": "LikedSongs",
    ///          "playlistType": 4,
    ///          "genreType": 4,
    ///          "songs": []
    ///      },
    ///      {
    ///          "id": 3,
    ///          "userId": "5f34130c-2ed9-4c83-a600-e474e8f48bac",
    ///          "title": "string",
    ///          "playlistType": 3,
    ///          "genreType": 0,
    ///          "songs": []
    ///      }
    ///     ]
    ///
    /// </remarks>
    /// <response code="200">If request is succeed. Also if preferable genre exists, but no matches for playlists  </response>
    /// <response code="404">If preferable genre doesn't exist.</response>
    [HttpGet("{genreType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRandomPlaylistsByGenre(GenreType genreType)
    {
        var playlistByGenre = await _ctx.GetRandomPlaylistsByGenre(genreType);
        if (playlistByGenre != null)
        {
            var result = playlistByGenre
                .Select(playlist => new
                {
                    playlist.Id, playlist.UserId, 
                    playlist.Title, playlist.PlaylistType, playlist.GenreType,
                    Songs = playlist.Songs.Select(sk => new
                    {
                        sk.Id, sk.UserId, sk.OriginPlaylistId, sk.Name, sk.Source
                    })
                });
            
            return new JsonResult(result);
        }
        
        return NotFound(new {Error = "not found"});
    }
}