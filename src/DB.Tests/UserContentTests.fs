module DB.Tests.UserContentTests

open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open DB

type responseName ={ name: string }
type responseNotFound ={ error: string }

[<Fact>]
let ``GetUserName returns real user's name by id 5f34130c-2ed9-4c83-a600-e474e8f48bac`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    
    let id = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let response = client.GetAsync($"/UserContent/name/user/{id}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseName> responseJson
    Assert.Equal("user01@gmail.com",responseData.name)
    
[<Fact>]
let ``GetUserName returns real user's name by id 5f34130c-2ed9-4c83-a600-e474e8f43bac`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    
    let id = "5f34130c-2ed9-4c83-a600-e474e8f43bac"
    let response = client.GetAsync($"/UserContent/name/user/{id}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseName> responseJson
    Assert.Equal("user03@gmail.com",responseData.name)
    
[<Fact>]
let ``GetUserName returns real user's name by id 5f34130c-2ed9-4c83-a600-e474e8f44bac`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    
    let id = "5f34130c-2ed9-4c83-a600-e474e8f44bac"
    let response = client.GetAsync($"/UserContent/name/user/{id}")
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseName> responseJson
    Assert.Equal("user04@gmail.com",responseData.name)
    
[<Fact>]
let ``GetUserName returns NotFound with description`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    
    let id = "bad id"
    let response = client.GetAsync($"/UserContent/name/user/{id}")
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseNotFound> responseJson
    Assert.Equal("Unexpected id",responseData.error)
    
[<Fact>]
let ``GetPlaylists returns NotFound with description`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    
    let id = "bad id"
    let response = client.GetAsync($"/UserContent/playlists/user/{id}")
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<responseNotFound> responseJson
    Assert.Equal("Unexpected id",responseData.error)
    
    
    