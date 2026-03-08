using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using GraphicsSecureCommunicationApi.WebApi.Data;
using GraphicsSecureCommunicationApi.WebApi.Mapping;
using GraphicsSecureCommunicationApi.WebApi.Mapping.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Repositories;
using GraphicsSecureCommunicationApi.WebApi.Repositories.Interfaces;
using GraphicsSecureCommunicationApi.WebApi.Services;
using GraphicsSecureCommunicationApi.WebApi.Services.Interfaces;
using System.Reflection;
using GraphicsSecureCommunicationApi.WebApi.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers for handling HTTP requests.
builder.Services.AddControllers();

// Retrieve the SQL connection string from configuration.
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// Register OpenAPI/Swagger for API documentation and testing.
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Graphics Secure Communication API",
        Version = "v1",
    });
});

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

// Register authorization services for securing endpoints.
builder.Services.AddAuthorization();

// Register ASP.NET Core Identity with Dapper stores for user authentication and management.
// Configures password and user requirements.
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddDapperStores(options =>
{
    options.ConnectionString = sqlConnectionString;
});

// Register IHttpContextAccessor for accessing HTTP context in services (e.g., to get current user info).
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Register mapping services
builder.Services.AddTransient<IEnvironment2DMappingService, Environment2DMappingService>();
builder.Services.AddTransient<IObject2DMappingService, Object2DMappingService>();

// Register DbContext for managing database connections
if (sqlConnectionStringFound)
{
    builder.Services.AddSingleton<IDbContext>(new DbContext(sqlConnectionString!));
}

// Register repositories
if (sqlConnectionStringFound)
{
    builder.Services.AddTransient<IEnvironment2DRepository, SqlEnvironment2DRepository>();
    builder.Services.AddTransient<IObject2DRepository, SqlObject2DRepository>();
}

// Register services
builder.Services.AddTransient<IEnvironment2DService, Environment2DService>();
builder.Services.AddTransient<IObject2DService, Object2DService>();

var app = builder.Build();

// Register OpenAPI/Swagger endpoints.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Graphics Secure Communication API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
        options.CacheLifetime = TimeSpan.Zero; // Disable caching for development

        // Inject a warning in the Swagger UI if the SQL connection string is missing
        if (!sqlConnectionStringFound)
            options.HeadContent = "<h1 align=\"center\">❌ SqlConnectionString not found ❌</h1>";
    });
}
else
{
    // Show the health message directly in non-development environments
    var buildTimeStamp = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
    string currentHealthMessage = $"The API is up 🚀 | Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")} | Build timestamp: {buildTimeStamp}";

    app.MapGet("/", () => currentHealthMessage);
}

// Enforce HTTPS for all requests.
app.UseHttpsRedirection();

// Enable authentication middleware.
app.UseAuthentication();

// Enable authorization middleware.
app.UseAuthorization();

// Register Identity endpoints for account management (register, login, etc.) under /account.
app.MapGroup("/account").MapIdentityApi<IdentityUser>().WithTags("Account");

// Register all controller endpoints for the application.
app.MapControllers();

app.Run();