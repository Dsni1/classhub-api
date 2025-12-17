using ClassHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new Exception("JWT_KEY environment variable is missing");

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? throw new Exception("JWT_ISSUER environment variable is missing");

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? throw new Exception("JWT_AUDIENCE environment variable is missing");



// JW configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// DB configuration from env variables on server
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");

string connectionString;

if (dbHost == null || dbPort == null || dbName == null || dbUser == null || dbPass == null)
{
    connectionString = $"Server=127.0.0.1;Database=classhub;User=devnet;Password=X9f!2nP@8dQm4L;Port=3306;Protocol=Tcp;SslMode=None;";
}

else
{
  connectionString =
    $"Server={dbHost};" +
    $"Port={dbPort};" +
    $"Database={dbName};" +
    $"User={dbUser};" +
    $"Password={dbPass};" +
    $"Protocol=Tcp;" +
    $"SslMode=None;";
  
}

var dbVersionString =builder.Configuration["Database:Version"];

var serverVersion = ServerVersion.Parse(dbVersionString);

Console.WriteLine("Using connection string: " + connectionString);

builder.Services.AddDbContext<ExternalDbContext>(options =>
{
    options.UseMySql(connectionString, serverVersion);
});

// Add services to the container.
builder.Services.AddScoped<ClassHub.Services.JwtService>();
builder.Services.AddScoped<ClassHub.Services.OrganisationInviteService>();
builder.Services.AddControllers();

// Build app
var app = builder.Build();

app.UseRouting();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


//final test commit