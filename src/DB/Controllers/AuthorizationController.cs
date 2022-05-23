using System.Security.Claims;
using DB.Attributes;
using DB.Infrastructure;
using DB.Data.Repository;
using DB.Models;
using DB.Models.Authorization;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants.Permissions;
using static OpenIddict.Server.AspNetCore.OpenIddictServerAspNetCoreConstants;


namespace DB.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthorizationController : ControllerBase
{
    private readonly SignInManager<UserInfo> _signInManager;
    private readonly UserManager<UserInfo> _userManager;
    private readonly IUserStore<UserInfo> _userStore;
    private readonly IUserEmailStore<UserInfo> _emailStore;
    private readonly ISpotifyRepository _ctx;

    public AuthorizationController(SignInManager<UserInfo> signInManager, 
        UserManager<UserInfo> userManager, IUserStore<UserInfo> userStore, ISpotifyRepository ctx)
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

    [AuthorizeWithJwt]
    [HttpPost("refresh_token")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> RefreshToken([FromForm] RefreshTokenData refreshTokenData)
    {
        var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [AuthorizeWithJwt]
    [HttpPost("validate_token")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> ValidateToken()
    {
        var claims = TokenHandler.GetClaims(Request);
        return Ok(claims);
    }

    [HttpPost("signup")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> SignUp([FromForm] PasswordFlowData passwordFlowData, [FromForm] ProfileData profileData)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request?.IsPasswordGrantType() == true)
        {
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

                var profile = new Profile()
                {
                    UserId = user.Id,
                    Username = profileData.Name ?? passwordFlowData.username,
                    Birthday = new DateOnly(profileData.BirthYear, profileData.BirthMonth, profileData.BirthDay),
                    Country = profileData.Country,
                    ProfileImg = profileData.ProfileImg,
                    UserType = UserType.User
                };
                await _ctx.CreateProfileAsync(profile);

                    var likedSongs = new Playlist()
                {
                    Title = "Liked Songs",
                    UserId = user.Id,
                    PlaylistType = PlaylistType.LikedSongs,
                    Verified = true
                };
                
                likedSongs.Users.Add(user);
                await _ctx.CreatePlaylist(likedSongs);

                await _ctx.Save();

                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                principal.SetScopes(new[]
                {
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles
                }.Intersect(request.GetScopes()));
                principal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess);

                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [Properties.ErrorDescription] =
                    "Unable to create new user"
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        var responseProperties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
            [Properties.ErrorDescription] =
                "The specified grant type is not implemented."
        });
        return Forbid(responseProperties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    }
    
    [HttpPost("login")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> LogIn([FromForm] PasswordFlowData passwordFlowData)
    {
        var request = HttpContext.GetOpenIddictServerRequest();
            if (request?.IsPasswordGrantType() == true)
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                {
                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    { 
                        [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [Properties.ErrorDescription] =
                            "The username/password couple is invalid."
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                
                var result = await _signInManager
                    .CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [Properties.ErrorDescription] =
                            "The username/password couple is invalid."
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                
                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                principal.SetScopes(new[]
                {
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles,
                }.Intersect(request.GetScopes()));
                principal.SetScopes(OpenIddictConstants.Scopes.OfflineAccess);

                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant type is not implemented.");
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
