module DB.Tests.NewHomePageTests

open System.Collections.Generic
open System.Net
open System.Net.Http.Headers
open System.Text.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open DB
open DB.Tests.AuthorizeUser

type ResponseToken = {access_token: string; token_type: string; expires_in: int}

[<Theory>]
[<InlineData(3, HttpStatusCode.OK)>]
[<InlineData(10, HttpStatusCode.OK)>]
[<InlineData(15, HttpStatusCode.OK)>]
[<InlineData(0, HttpStatusCode.BadRequest)>]
let ``Authorized get random playlists`` (count: int, statusCode: HttpStatusCode) =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let token = AuthorizeUser.access_token
    client.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", token)
    let response = client.GetAsync($"/api/home/random/playlists?count={count}")
    Assert.Equal(statusCode, response.Result.StatusCode)
    if response.Result.StatusCode = HttpStatusCode.OK
    then
        let responseJson = response.Result.Content.ReadAsStringAsync().Result
        let responseData = JsonSerializer.Deserialize<List<DB.Models.Playlist>> responseJson
        Assert.NotNull(responseData)
    
[<Theory>]
[<InlineData(3, HttpStatusCode.Unauthorized)>]
[<InlineData(10, HttpStatusCode.Unauthorized)>]
[<InlineData(15, HttpStatusCode.Unauthorized)>]
[<InlineData(0, HttpStatusCode.Unauthorized)>]
let ``Unauthorized get random playlists`` (count: int, statusCode: HttpStatusCode) =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let response = client.GetAsync($"/api/home/random/playlists?count={count}")
    Assert.Equal(statusCode, response.Result.StatusCode)