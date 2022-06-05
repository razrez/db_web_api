using DB.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DB.Controllers;

[ApiController]
[Route("api/user/content")]
[Produces("application/json")]
public class UserContentController : ControllerBase
{
    private readonly ISpotifyRepository _repository; 

    public UserContentController(ISpotifyRepository repository)
    {
        _repository = repository;
    }
    
    //5f34130c-2ed9-4c83-a600-e474e8f48bac
    /// <summary>
    /// Gets user's name.
    /// </summary>
    /// <remarks>
    /// Sample response:
    ///
    ///     {
    ///        "name": "username",
    ///     }
    ///
    /// </remarks>
    /// <param name="userId"></param>
    /// <response code="200">If request is succeed.</response>
    /// <response code="404">If user with preferable ID doesn't exist.</response>
    [HttpGet]
    [Route("name/{userId}", Name="GetUserName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserName(string userId)
    {
        try
        {
            var name = await _repository.GetUserName(userId);
            return new JsonResult(new {Name = name});
        }
        catch (Exception)
        {
            return NotFound(new {Error = "Unexpected id"});
        }
        
    }
    
    //5f34130c-2ed9-4c83-a600-e474e8f48bac
    /// <summary>
    /// Gets all playlists that were created by the user.
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
    /// <response code="404">If user with preferable ID doesn't exist.</response>
    [HttpGet]
    [Route("playlists/{userId}", Name="GetUsersPlaylists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlaylists(string userId)
    {
        var user = await _repository.FindUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new {Error = "Unexpected id"});
        }
        
        //свяжем для примера имеющиеся в бд песни с плейлистами, плейлисты с пользователем
        //лайкнем самую первую песню
        //один раз использовал - закоммитить можно
        /*var isLiked = await _repository.LikeSong(2, userId);
        if (!isLiked) return NotFound();*/
        
        var usersPlaylists = _repository.GetUsersPlaylists(userId).Result
            .Select(playlist => new
            {
                playlist.Id, playlist.UserId, playlist.Title, 
                playlist.PlaylistType, playlist.GenreType,
                Songs = playlist.Songs.Select(sk => new
                {
                    sk.Id, sk.UserId, sk.Name, sk.Source
                })
            });
        
        return new JsonResult(usersPlaylists);
    }

}
