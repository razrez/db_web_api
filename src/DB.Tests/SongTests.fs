module DB.Tests.SongTests

open System.Net
open Microsoft.AspNetCore.Mvc.Testing
open System.Text.Json
open Xunit
open DB
open GetResponse

[<Theory>]
[<InlineData("1")>]
[<InlineData("2")>]
[<InlineData("3")>]

let ``Search Song returns Song``(songId: string) =
    let path = $"/api/song/getSong?songId={songId}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<DB.Models.Song> responseJson
    Assert.NotNull(responseData)
    
    
[<Theory>]
[<InlineData("NotExistingName")>]
[<InlineData("qwerty123456")>]
[<InlineData("")>]
let ``Search Song return Not Found``(songId: string) =
    let path = $"/api/song/getSong?songId={songId}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    
    
[<Fact>]
let ``Successfully Adding a Song to a Playlist``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let songId = 2
    let playlistId = 2
    
    let response = client.PostAsync($"/api/song/addSongToPlaylist/{songId},{playlistId}", null)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
[<Fact>]
let ``Error Adding a Song to a Playlist``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let songId = 999999
    let playlistId = 999999
    
    let response = client.PostAsync($"/api/song/addSongToPlaylist/{songId},{playlistId}", null)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)