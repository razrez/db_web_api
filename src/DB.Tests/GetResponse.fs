module DB.Tests.GetResponse

open Microsoft.AspNetCore.Mvc.Testing
open DB

let getResponseAsync path=
    let factory = new WebApplicationFactory<Startup>()
    let client = factory.CreateClient()
    client.GetAsync($"{path}")