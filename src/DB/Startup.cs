using System.Reflection;
using DB.Data;
using DB.Data.Repository;
using DB.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


namespace DB;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<SpotifyContext>(options =>
        {
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
            options.UseOpenIddict();
        });
        services.AddIdentity();
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        services.AddAuthenticationAndJwt(_configuration)
            .AddAuthorization()
            .AddOpenIddictServer(_env);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
                option.SchemaFilter<EnumSchemaFilter>();
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Spotify Web API",
                    Description = "ASP.NET Core Web API that allows you work with Spotify app data.",
                    Contact = new OpenApiContact
                    {
                        Name = "Team: ozon671development",
                        Url = new Uri("https://github.com/razrez/db_web_api"),
                    },
                    Version = "v1"
                });
                
                //я сгенерировал и закоммитил, потому что из-за рефлексии падают тесты
                //так как фшарп не может просто создать копию Startup из-за кода ниже
                //если захотели написать документацию, то раскоммитьте снизу, чтобы сгенерировать
                
                /*
                //generate XML docs
                var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                option.IncludeXmlComments(xmlPath);
                */
                
                option.IncludeXmlComments("DB.xml");
            }
        );
        
        services.AddScoped<ISpotifyRepository, SpotifyRepository>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger";
            });
        }
        app
            .UseStaticFiles() //for wwwroot
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseCors()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) => builder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables())
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    public static void RunApp(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
}