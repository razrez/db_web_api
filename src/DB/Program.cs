using System.Text.Json.Serialization;
using DB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SpotifyContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddIdentity<UserInfo, IdentityRole>()
    .AddEntityFrameworkStores<SpotifyContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>    //claims настрою при реализации авторизации
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
});

builder.Services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder            //Позже настроим CORS на принятие запросов только с нашего приложения
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

builder.Services.AddAuthentication(configureOptions =>
    {
        configureOptions.DefaultAuthenticateScheme = AuthenticationScheme;
        configureOptions.DefaultChallengeScheme = AuthenticationScheme;
    })
    .AddJwtBearer(options => { options.ClaimsIssuer = AuthenticationScheme; });
builder.Services.AddAuthorization();


builder.Services.AddOpenIddict()
    .AddCore(coreOptions =>
    {
        coreOptions
            .UseEntityFrameworkCore()
            .UseDbContext<SpotifyContext>();
    })
    .AddServer(serverOptions =>
    {
        serverOptions
            .AcceptAnonymousClients()
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow();

        serverOptions
            .SetTokenEndpointUris("/authenticate"); //метод аутентификации и выдачи JWT

        var cfg = serverOptions.UseAspNetCore();
        if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
        {
            cfg.DisableTransportSecurityRequirement();   
        }
        cfg.EnableTokenEndpointPassthrough();

        serverOptions
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
    })
    .AddValidation(validationOption =>
    {
        validationOption.UseAspNetCore();
        validationOption.UseLocalServer();
    });

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .UseStaticFiles()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseCors()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapDefaultControllerRoute();
    });
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
