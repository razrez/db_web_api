module DB.Tests.AuthorizeUser

open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text.Json
open Microsoft.AspNetCore.Mvc.Testing
open DB
open DB.Tests.ResponseToken

let AuthorizeUser =
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
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<ResponseToken> responseJson
    responseData.access_token
