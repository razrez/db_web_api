using DB.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class UserContentController : ControllerBase
{
    private readonly ISpotifyRepository _repository; //чтобы всёе краш лось отдельно добавлю, пока метода не вынес

    public UserContentController(ISpotifyRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [Route("name/user/{userId}", Name="GetUserName")]
    public async Task<IActionResult> GetUserName(string userId)
    {
        var name = await _repository.GetUserName(userId);
        if (name.Length != 0)
        {
            return new JsonResult(new {Name = name});
        }
        return NotFound(new {Error = "Unexpected id"});
    }
    
    //5f34130c-2ed9-4c83-a600-e474e8f48bac
    [HttpGet]
    [Route("playlists/user/{userId}", Name="GetUsersPlaylists")]
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
        var isLiked = await _repository.LikeSong(2, userId);
        if (!isLiked) return NotFound();
        var usersPlaylists = _repository.GetUsersPlaylists(userId).Result
            .Select(s => new
            {
                s.Id, s.UserId, s.Title, s.PlaylistType,
                Songs = s.Songs.Select(sk => new
                {
                    sk.Id, sk.UserId, sk.Name, sk.Source
                })
            });
        
        return new JsonResult(usersPlaylists);
    }

}
