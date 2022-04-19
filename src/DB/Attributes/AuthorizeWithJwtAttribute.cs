using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace DB.Attributes;

public class AuthorizeWithJwtAttribute : AuthorizeAttribute
{
    public AuthorizeWithJwtAttribute()
    { 
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    }
}
