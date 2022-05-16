module DB.Tests.SearchPageTests

open System.Net
open Microsoft.AspNetCore.Mvc.Testing
open System.Text.Json
open Xunit
open DB


[<Theory>]
[<InlineData("Liked")>]
[<InlineData("playlist")>]
[<InlineData("LIKED")>]
[<InlineData("pLayLIst")>]
let ``Search Playlists return Playlists``(input: string) =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let response = client.GetAsync($"/api/search/playlists?input={input}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<List<DB.Models.Playlist>> responseJson
    Assert.NotNull(responseData)
    
[<Theory>]
[<InlineData("song")>]
[<InlineData("SONG")>]
[<InlineData("song1")>]
[<InlineData("SoNg1")>]
let ``Search Songs return Songs``(input: string) =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let response = client.GetAsync($"/api/search/songs?input={input}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<List<DB.Models.Song>> responseJson
    Assert.NotNull(responseData)

[<Theory>]
[<InlineData("user")>]
[<InlineData("USER")>]
[<InlineData("user02")>]
[<InlineData("UsEr02")>]
let ``Search Users return Users``(input: string) =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let response = client.GetAsync($"/api/search/users?input={input}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<List<DB.Models.Profile>> responseJson
    Assert.NotNull(responseData)
  
[<Theory>]
[<InlineData("artist")>]
[<InlineData("ARTIST")>]
[<InlineData("artist02")>]
[<InlineData("ArTisT02")>]
let ``Search Artists return Artists``(input: string) =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let response = client.GetAsync($"/api/search/artists?input={input}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<List<DB.Models.Profile>> responseJson
    Assert.NotNull(responseData)
    

