
module DB.Tests.PlaylistTests

open System.Net
open System.Text.Json
open DB
open DB.Controllers
open DB.Data.Repository
open DB.Models
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
let ``GetPlaylist by id return Playlist``() =
    let id = 1
    let factory = new WebApplicationFactory<Startup>()
    let client = factory.CreateClient()
    
    let path = $"/api/playlist/{id}"
    let response = client.GetAsync($"{path}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    Assert.NotEmpty(responseJson)
    let responseData = JsonSerializer.Deserialize<playlist> responseJson
    Assert.Equal("LikedSongs", responseData.title)