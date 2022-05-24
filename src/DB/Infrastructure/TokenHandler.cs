using System.IdentityModel.Tokens.Jwt;
using DB.Models.Authorization;
using Microsoft.Net.Http.Headers;

namespace DB.Infrastructure;

public class TokenHandler
{
    private static string GetToken(HttpRequest request)
    {
        var token = request.Headers[HeaderNames.Authorization]
            .ToString()
            .Replace("Bearer ", "");
        return token;
    }

    public static UserClaims GetClaims(HttpRequest request)
    {
        var token = GetToken(request);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var claims = new UserClaims()
        {
            Id = jwt.Claims.First(claim => claim.Type == "sub").Value,
            Name = jwt.Claims.First(claim => claim.Type == "name").Value,
            Role = jwt.Claims.First(claim => claim.Type == "role").Value,
        };
        return claims;
    }
        
}