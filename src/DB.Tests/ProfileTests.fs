module DB.Tests.ProfileTests

open System.Net
open DB.Tests.UserContentTests
open Microsoft.AspNetCore.Mvc
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
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let userId = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let username = "user01@gmail.com"
    let birthday = "2000.01.01"
    let email = "user01@gmail.com"
    let country = Country.Greece
    
    let response = client.PostAsync($"/api/profile/changeProfile/{userId}, {username}, {country}, {birthday}, {email}", null)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
    
[<Fact>]
let ``Change Profile returns NotFound``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let userId = "noProfile"
    let username = "name"
    let birthday = "1.1.1"
    let email = "mail"
    let country = Country.Greece
    
    let response = client.PostAsync($"/api/profile/changeProfile/{userId}, {username}, {country}, {birthday}, {email}", null)
    Assert.Equal(HttpStatusCode.NotFound, response.Result.StatusCode)
    
    
[<Fact>]
let ``Change Password returns Success``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let userId = "5f34130c-2ed9-4c83-a600-e474e8f44bac"
    let oldPassword = "qWe!123"
    let newPassword = "newqWe!123"
    
    let response = client.PostAsync($"/api/profile/changePassword/{userId},{oldPassword}, {newPassword}", null)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    

[<Fact>]
let ``Change Password returns Password Wrong``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let userId = "5f34130c-2ed9-4c83-a600-e474e8f43bac"
    let oldPassword = "noPassword"
    let newPassword = "qWe!123"
    
    let response = client.PostAsync($"/api/profile/changePassword/{userId},{oldPassword}, {newPassword}", null)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    
[<Fact>]
let ``Change Password returns NotFound``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient();
    let userId = "noUser"
    let oldPassword = "qWe!123"
    let newPassword = "newqWe!123"
    
    let response = client.PostAsync($"/api/profile/changePassword/{userId},{oldPassword}, {newPassword}", null)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    
    
[<Fact>]
let ``Change Premium returns Success``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let userId = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let premiumType = PremiumType.Duo
    let response = client.PostAsync($"/api/profile/changePremium/{userId},{premiumType}", null)
    Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode)
    
[<Fact>]
let ``Change Premium returns Already such a Premium``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let userId = "5f34130c-2ed9-4c83-a600-e474e8f48bac"
    let premiumType = PremiumType.Duo
    let response = client.PostAsync($"/api/profile/changePremium/{userId},{premiumType}", null)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    
    
[<Fact>]
let ``Change Premium returns NotFound``() =
    let _factory = new WebApplicationFactory<Startup>()
    let client = _factory.CreateClient()
    let userId = "noUser"
    let premiumType = PremiumType.Duo
    let response = client.PostAsync($"/api/profile/changePremium/{userId},{premiumType}", null)
    Assert.Equal(HttpStatusCode.BadRequest, response.Result.StatusCode)
    
    