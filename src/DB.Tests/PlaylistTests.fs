module DB.Tests.PlaylistTests

open System
open System.Linq.Expressions
open System.Net
open System.Text.Json
open DB.Data.Repository
open Microsoft.FSharp.Collections
open Moq
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open DB

type responseName = { name: string }
type responseNotFound = { error: string }
type song = {id: int; userId: string; name:string; source: string}
type playlist = { id: int; userId: string; title: string; playlistType:int; genreType:int; songs:List<song> }
type responsePlaylists = List<playlist>
type Moq.Mock<'T> when 'T : not struct with
          /// Specifies a setup on the mocked type for a call to a function
          member mock.SetupFunc<'TResult>(expression:Expression<Func<'T,'TResult>>) =
            mock.Setup<'TResult>(expression)
          /// Specifies a setup on the mocked type for a call to a void method
          member mock.SetupAction(expression:Expression<Action<'T>>) = 
            mock.Setup(expression)
          /// Specifies a setup on the mocked type for a call to a property setter
          member mock.SetupSetAction<'TProperty>(setupExpression:Action<'T>) 
            : Moq.Language.Flow.ISetupSetter<'T,'TProperty> = 
            mock.SetupSet<'TProperty>(setupExpression)
            
let createClient =
    let factory = new WebApplicationFactory<Startup>()
    factory.CreateClient()
    
[<Fact>]
let ``Delete nonexistent Playlist and get NotFound`` () =
    let id = 234567892
    
    let mock = Mock.Of<ISpotifyRepository>().DeletePlaylist(id).Result
    (*let controller  = PlaylistController(mock)
    let actionResult = controller.DeletePlaylist(id).Result.ToString()*)
    Assert.False(mock)
    
    let path id = $"/api/playlist/{id}"
    let client = createClient
    let response = client.DeleteAsync($"{path}")
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
    
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseNotFound> responseJson
    Assert.Equal("not found",responseData.error)
    

    