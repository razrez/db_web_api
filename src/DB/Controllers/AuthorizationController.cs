namespace DB.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly SignInManager<UserInfo> _signInManager;
    private readonly UserManager<UserInfo> _userManager;
    private readonly IUserStore<UserInfo> _userStore;
    private readonly IUserEmailStore<UserInfo> _emailStore;
    private readonly SpotifyContext _ctx;

    public AuthorizationController(SignInManager<UserInfo> signInManager, 
        UserManager<UserInfo> userManager, IUserStore<UserInfo> userStore, SpotifyContext ctx)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _ctx = ctx;
        _emailStore = GetEmailStore();
    }
    
    private IUserEmailStore<UserInfo> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<UserInfo>)_userStore;
    }
    
    [HttpPost("~/signup"), Produces("application/json")]
    public async Task<IActionResult> SignUp(string profileJson)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        
        var user = new UserInfo();

        await _userStore.SetUserNameAsync(user, request?.Username, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, request?.Username, CancellationToken.None);
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request?.Password);
        var result = await _userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, code);
            
            var profile = JsonConvert.DeserializeObject<Profile>(profileJson);
            if (profile != null)
            {
                profile.UserId = user.Id;
                await _ctx.Profiles.AddAsync(profile);
                await _ctx.SaveChangesAsync();
            }
            
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            
            principal.SetScopes(new[]
            {
                Scopes.Email,
                Scopes.Profile,
                Scopes.Roles
            }.Intersect(request.GetScopes()));
            
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }
            
            await _signInManager.SignInAsync(user, isPersistent: false);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        foreach(IdentityError error in result.Errors)
            Console.WriteLine($"Oops! {error.Description} ({error.Code})");

        throw new NotImplementedException("Unable to create new user");
    }
    
    private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(Scopes.Profile))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(Scopes.Email))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                    yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }
}
