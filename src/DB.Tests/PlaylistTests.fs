module DB.Tests.PlaylistTests

open System
open System.Linq.Expressions
open System.Net
open DB.Data
open DB.Data.Repository
open Moq
open Microsoft.AspNetCore.Mvc.Testing
open Moq.EntityFrameworkCore
open Xunit
open DB

type responseName = { name: string }
type responseNotFound = { error: string }
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
            
let deleteResponseAsync path=
    let factory = new WebApplicationFactory<Startup>()
    let client = factory.CreateClient()
    client.DeleteAsync($"{path}")
    
type Container<'a> = {repo: 'a;}

[<Fact>]
let ``Delete nonexistent Playlist and get NotFound`` () =
    let id = 234567892
    
    let mock = Mock.Of<ISpotifyRepository>().DeletePlaylist(id).Result
    Assert.False(mock)
    (*let repMoq = Moq.Mock<ISpotifyRepository>()
                     .SetupFunc<bool>(fun f -> f.DeletePlaylist(It.IsAny<int>()).Result).Returns(false)*)
    let path id = $"/api/playlist/{id}"
    let response = deleteResponseAsync path
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)

    