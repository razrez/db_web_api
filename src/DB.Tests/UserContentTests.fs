module DB.Tests.UserContentTests

open System.Collections.Generic
open System.Net
open System.Text.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open DB

type responseName = { name: string }
type responseNotFound = { error: string }
type song = {id: int; userId: string; name:string; source: string}
type playlist = { id: int; userId: string; title: string; playlistType:int; genreType:int; songs:List<song> }
type responsePlaylists = List<playlist>

let getResponseAsync path=
    let factory = new WebApplicationFactory<Startup>()
    let client = factory.CreateClient()
    client.GetAsync($"{path}")
    
[<Theory>]
[<InlineData("5f34130c-2ed9-4c83-a600-e474e8f48bac","user01@gmail.com")>]
[<InlineData("5f34130c-2ed9-4c83-a600-e474e8f43bac","user03@gmail.com")>]
[<InlineData("5f34130c-2ed9-4c83-a600-e474e8f44bac","user04@gmail.com")>]
let ``GetUserName returns real user's name by id 5f34130c-2ed9-4c83-a600-e474e8f48bac`` (id:string, expected:string) =
    let path = $"/UserContent/name/user/{id}"
    
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseName> responseJson
    Assert.Equal(expected,responseData.name)
    

[<Fact>]
let ``GetUserName returns NotFound with description`` () =
    let id = "bad id"
    let path = $"/UserContent/name/user/{id}"
    
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseNotFound> responseJson
    Assert.Equal("Unexpected id",responseData.error)
    
[<Fact>]
let ``GetPlaylists returns NotFound with description`` () =
    let id = "bad id"
    let path = $"/UserContent/playlists/user/{id}"
    
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseNotFound> responseJson
    Assert.Equal("Unexpected id",responseData.error)
    
[<Fact>]
let ``GetPlaylists returns not empty user's playlists by id 5f34130c-2ed9-4c83-a600-e474e8f48bac`` () =
    let id = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let path = $"/UserContent/playlists/user/{id}"
    
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    Assert.NotEmpty(responseJson)
    
    let responseData = JsonSerializer.Deserialize<responsePlaylists> responseJson
    Assert.Equal(2,responseData.Count)
    
    
    