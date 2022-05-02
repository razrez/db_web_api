using DB.Data;
using DB.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using static Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults;

namespace DB;

public static class StartupExtensions
{
    public static IServiceCollection AddAuthenticationAndJwt(this IServiceCollection sc)
    {
        sc.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultAuthenticateScheme = AuthenticationScheme;
                configureOptions.DefaultChallengeScheme = AuthenticationScheme;
            })
            .AddJwtBearer(options => { options.ClaimsIssuer = AuthenticationScheme; });
        return sc;
    }
    
    public static OpenIddictBuilder AddOpenIddictServer(this IServiceCollection services, 
        IWebHostEnvironment environment)
    {
        return services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options
                    .UseEntityFrameworkCore()
                    .UseDbContext<SpotifyContext>();
            })
            .AddServer(options =>
            {
                options
                    .AcceptAnonymousClients()
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow();

                options
                    .SetTokenEndpointUris("/signup", "/login");
                
                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                var cfg = options.UseAspNetCore();
                if (environment.IsDevelopment() || environment.IsStaging())
                {
                    cfg.DisableTransportSecurityRequirement();   
                }
                
                cfg.EnableTokenEndpointPassthrough();
                
                options
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();
            }).AddValidation(options =>
            {
                options.UseAspNetCore();
                options.UseLocalServer();
            });
    }
    
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<UserInfo, IdentityRole>()
            .AddEntityFrameworkStores<SpotifyContext>()
            .AddDefaultTokenProviders();
        
        // Configure Identity to use the same JWT claims as OpenIddict instead
        // of the legacy WS-Federation claims it uses by default (ClaimTypes),
        // which saves you from doing the mapping in your authorization controller.
        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
        });
        
        return services;
    }
}