module DB.Tests.AuthorizationTests

open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text.Json
open Microsoft.AspNetCore.Mvc.Testing
open Xunit
open DB
open DB.Tests.ResponseToken

type ResponseError = {error: string; error_description: string; error_uri: string}

[<Fact>]
let ``Correct Sign Up returns JWT`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let values = [|
        KeyValuePair<string, string>("grant_type", "password");
        KeyValuePair<string, string>("username", "Admin10@gmail.com");
        KeyValuePair<string, string>("password", "AsdQwe-123");
    |]
    let content = new FormUrlEncodedContent(values)
    content.Headers.ContentType <- Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
    let profile = "{\"UserId\":\"\",\"Username\":\"user\",\"Birthday\":null,\"Country\":1,\"ProfileImg\":null,\"UserType\":0,\"User\":null}"
    let response = client.PostAsync($"/signup?profileJson={profile}", content)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<ResponseToken> responseJson
    Assert.NotNull(responseData.access_token)
    
[<Fact>]
let ``Sign Up with wrong data return BadRequest`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let values = [|
        KeyValuePair<string, string>("grant_type", "password");
        KeyValuePair<string, string>("username", "user01@gmail.com");
        KeyValuePair<string, string>("password", "qWe!123");
    |]
    let content = new FormUrlEncodedContent(values)
    content.Headers.ContentType <- Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
    let profile = "{\"UserId\":\"\",\"Username\":\"user\",\"Birthday\":null,\"Country\":1,\"ProfileImg\":null,\"UserType\":0,\"User\":null}"
    let response = client.PostAsync($"/signup?profileJson={profile}", content)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<ResponseError> responseJson
    Assert.Equal("Unable to create new user", responseData.error_description)
    
[<Fact>] 
let ``Correct Login returns JWT`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let values = [|
        KeyValuePair<string, string>("grant_type", "password");
        KeyValuePair<string, string>("username", "user01@gmail.com");
        KeyValuePair<string, string>("password", "qWe!123");
    |]
    let content = new FormUrlEncodedContent(values)
    content.Headers.ContentType <- Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
    let response = client.PostAsync($"/login", content)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<ResponseToken> responseJson
    Assert.NotNull(responseData.access_token)

[<Fact>]
let ``Log In with wrong user name return BadRequest`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let values = [|
        KeyValuePair<string, string>("grant_type", "password");
        KeyValuePair<string, string>("username", "user11@gmail.com");
        KeyValuePair<string, string>("password", "qWe!123");
    |]
    let content = new FormUrlEncodedContent(values)
    content.Headers.ContentType <- Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
    let response = client.PostAsync($"/login", content)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<ResponseError> responseJson
    Assert.Equal("The username/password couple is invalid.", responseData.error_description)
    
[<Fact>]
let ``Log In with wrong password return BadRequest`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let values = [|
        KeyValuePair<string, string>("grant_type", "password");
        KeyValuePair<string, string>("username", "user01@gmail.com");
        KeyValuePair<string, string>("password", "qWe!123A");
    |]
    let content = new FormUrlEncodedContent(values)
    content.Headers.ContentType <- Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
    let response = client.PostAsync($"/login", content)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<ResponseError> responseJson
    Assert.Equal("The username/password couple is invalid.", responseData.error_description)
