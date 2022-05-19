
module DB.Tests.PlaylistTests

open System.Collections.Generic
open System.Net
open System.Text.Json
open DB
open DB.Controllers
open DB.Data.Repository
open DB.Models
open DB.Models.EnumTypes
open DB.Tests.UserContentTests
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Testing
open Moq
open Moq.FSharp.Extensions
open Xunit
open DB.Tests.GetResponse

(*type Moq.Mock<'T> when 'T : not struct with
          /// Specifies a setup on the mocked type for a call to a function
          member mock.SetupFunc<'TResult>(expression:Expression<Func<'T,'TResult>>) =
            mock.Setup<'TResult>(expression)
          /// Specifies a setup on the mocked type for a call to a void method
          member mock.SetupAction(expression:Expression<Action<'T>>) = 
            mock.Setup(expression)
          /// Specifies a setup on the mocked type for a call to a property setter
          member mock.SetupSetAction<'TProperty>(setupExpression:Action<'T>) 
            : Moq.Language.Flow.ISetupSetter<'T,'TProperty> = 
            mock.SetupSet<'TProperty>(setupExpression)*)
type IRepository =
    inherit ISpotifyRepository
    
type song = {id: int; userId: string; name:string; source: string}
type playlist = { id: int; userId: string; title: string; playlistType:int; genreType:int; songs:List<song> }
    
[<Fact>]
let ``Delete nonexistent Playlist and get NotFound`` () =
    let id = 234567892
    
    let path id = $"/api/playlist/{id}"
    let response = deleteResponseAsync path
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.DeletePlaylist(It.IsAny<int>()).Result).Returns(false) |> ignore
    
    Assert.False(moq.Object.DeletePlaylist(id).Result)
    let playlistController = PlaylistController(moq.Object)
    
    let myActionResult = playlistController.DeletePlaylist(id).Result
    Assert.IsType<NotFoundObjectResult>(myActionResult)
    
[<Fact>]
let ``Delete existent Playlist and get Ok`` () =
    let id = 2
    
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.DeletePlaylist(It.IsAny<int>()).Result).Returns(true) |> ignore
    Assert.True(moq.Object.DeletePlaylist(id).Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.DeletePlaylist(id).Result
    Assert.IsType<OkResult>(myActionResult)
    
[<Fact>]
let ``GetPlaylist by valid user's id returns Playlist``() =
    let id = 1
    let path = $"/api/playlist/{id}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    Assert.NotEmpty(responseJson)
    let responseData = JsonSerializer.Deserialize<playlist> responseJson
    Assert.Equal("LikedSongs", responseData.title)

[<Fact>]    
let ``GetPlaylist by wrong id returns NotFound``() =
    let id = -231
    let factory = new WebApplicationFactory<Startup>()
    let client = factory.CreateClient()
    
    let path = $"/api/playlist/{id}"
    let response = client.GetAsync($"{path}")
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
[<Fact>]    
let ``Edit Playlists and get OK``() =
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.EditPlaylist(It.IsAny<Playlist>()).Result).Returns(true) |> ignore
    
    let editedPlaylist = Playlist(
                            Id = 2,
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                            Title ="simple playlist changed title",PlaylistType = PlaylistType.User,
                            GenreType = GenreType.Electro,
                            ImgSrc = "new image url",
                            Verified = true)
    Assert.True(moq.Object.EditPlaylist(editedPlaylist).Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.EditPlaylist(editedPlaylist).Result
    Assert.IsType<OkResult>(myActionResult)
    
[<Fact>]    
let ``Edit nonexistent Playlists and get BadRequest``() =
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.EditPlaylist(It.IsAny<Playlist>()).Result).Returns(false) |> ignore
    
    let editedPlaylist = Playlist(
                            Id = -213122,
                            UserId = "clickclickclick",
                            Title ="simple playlist changed title",PlaylistType = PlaylistType.User,
                            GenreType = GenreType.Electro, ImgSrc = "new image url", Verified = true)
    Assert.False(moq.Object.EditPlaylist(editedPlaylist).Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.EditPlaylist(editedPlaylist).Result
    Assert.IsType<BadRequestObjectResult>(myActionResult)
    
[<Fact>]    
let ``Create Playlist and get OK``() =
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.CreatePlaylist(It.IsAny<Playlist>()).Result).Returns(true) |> ignore
    
    let editedPlaylist = Playlist(
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                            Title ="simple playlist changed title",PlaylistType = PlaylistType.User,
                            GenreType = GenreType.Electro, ImgSrc = "new image url", Verified = true)
    Assert.True(moq.Object.CreatePlaylist(editedPlaylist).Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.CreatePlaylist(editedPlaylist).Result
    Assert.IsType<OkResult>(myActionResult)

[<Fact>]    
let ``Create Playlist and get BadRequest``() =
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.CreatePlaylist(It.IsAny<Playlist>()).Result).Returns(false) |> ignore
    
    let editedPlaylist = Playlist(
                            UserId = "5sadasdas ds",
                            Title ="simple playlist changed title")
    Assert.False(moq.Object.CreatePlaylist(editedPlaylist).Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.CreatePlaylist(editedPlaylist).Result
    Assert.IsType<BadRequestObjectResult>(myActionResult)
    
[<Fact>]    
let ``Like Playlist and get Ok``() =
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.LikePlaylist(It.IsAny<int>(),It.IsAny<string>()).Result).Returns(true) |> ignore
    
    Assert.True(moq.Object.LikePlaylist(2,"5f34130c-2ed9-4c83-a600-e474e8f48bac").Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.LikePlaylist(2, "5f34130c-2ed9-4c83-a600-e474e8f48bac").Result
    Assert.IsType<OkResult>(myActionResult)
    
[<Fact>]    
let ``Like Playlist and get BadRequest``() =
    let moq = Mock<IRepository>()
    moq.SetupFunc<bool>(fun f -> f.LikePlaylist(It.IsAny<int>(),It.IsAny<string>()).Result).Returns(false) |> ignore
    
    Assert.False(moq.Object.LikePlaylist(-123,"adasdsad").Result)
    
    let playlistController = PlaylistController(moq.Object)
    let myActionResult = playlistController.LikePlaylist(-32, "sadsadsafeewx").Result
    Assert.IsType<BadRequestObjectResult>(myActionResult)
    
[<Fact>]    
let ``GetLibrary by wrong id returns NotFound``() =
    let id = "sadas-sad11cs" 
    let path = $"/api/playlist/library/user/{id}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
[<Fact>]
let ``GetLibrary by valid user's id returns List of liked playlists``() =
    let id = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let path = $"/api/playlist/library/user/{id}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    (*let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseLibrary = JsonSerializer.Deserialize<responsePlaylists> responseJson
    Assert.Empty(responseLibrary)*)