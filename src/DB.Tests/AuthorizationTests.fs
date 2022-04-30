module DB.Tests.AuthorizationTests
     
open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open Microsoft.AspNetCore.Mvc.Testing
open Microsoft.Net.Http.Headers
open Xunit
open DB



[<Fact>]
let ``Correct Sign Up`` () =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let values = [|
        KeyValuePair<string, string>("grant_type", "password");
        KeyValuePair<string, string>("username", "Admin126@gmail.com");
        KeyValuePair<string, string>("password", "AsdQwe-123");
    |]
    let content = new FormUrlEncodedContent(values)
    content.Headers.ContentType <- Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
    let profile = "{\"UserId\":\"\",\"Username\":\"user\",\"Birthday\":null,\"Country\":1,\"ProfileImg\":null,\"UserType\":0,\"User\":null}"
    let response = client.PostAsync($"/signup?profileJson={profile}", content)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
   
[<Fact>] 
let ``Correct Login`` () =
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