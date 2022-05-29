module DB.Tests.ProfileTests

open System.Net
open DB.Tests.UserContentTests
open Microsoft.AspNetCore.Mvc.Testing
open System.Text.Json
open Xunit
open DB
open DB.Models.EnumTypes
open GetResponse

type responseNotFound = { error: string }
type profile = {userId:string; username:string; birthday:string; email:string; country:Country}
type responseProfile = List<profile>

[<Theory>]
[<InlineData("5f34130c-2ed9-4c83-a600-e474e8f48bac")>]
[<InlineData("ca2aa01b-a215-4611-838a-f11b9552103e")>]
[<InlineData("5f34130c-2ed9-4c83-a600-e474e8f43bac")>]
let ``Search Profile returns Profile``(userId: string) =
    let path = $"/api/profile/getProfile?userId={userId}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    let responseJson = response.Result.Content.ReadAsStringAsync().Result
    let responseData = JsonSerializer.Deserialize<DB.Models.Profile> responseJson
    Assert.NotNull(responseData)
    
    
[<Theory>]
[<InlineData("NotExistingName")>]
[<InlineData("qwerty123456")>]
[<InlineData("")>]
let ``Search Profile return Not Found``(userId: string) =
    let path = $"/api/profile/getProfile?input={userId}"
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    
    
    
[<Fact>]
let ``Change Profile returns Profile``() =
    let userId = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let username = "user01@gmail.com"
    let birthday = "2000.01.01"
    let email = "user01@gmail.com"
    let country = Country.Greece
    let path = $"/api/profile/changeProfile?{userId}, {username}, {country} {birthday}, {email}"
    
    let response = getResponseAsync path
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    
    

