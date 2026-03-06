using System.Text;
using Api.Auth;
using Api.Data;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// configuration from environment
builder.Configuration.AddEnvironmentVariables();

// services
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
                  ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");
    if (string.IsNullOrEmpty(connStr))
        throw new InvalidOperationException("Connection string not configured");

    // simple provider selection: if contains host= assume PostgreSQL
    if (connStr.Contains("Host=") || connStr.Contains("host="))
    {
        options.UseNpgsql(connStr);
    }
    else
    {
        options.UseSqlite(connStr);
    }
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        // if you ever need credentials across origins, use:
        // policy.AllowCredentials();
    });
});

// authentication
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!
                  ?? throw new InvalidOperationException("Jwt options not configured");

var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Console.WriteLine("Ensuring database is created...");
    var created = dbContext.Database.EnsureCreated();
    Console.WriteLine($"Database creation result: {created}");
}

// middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // apply named policy
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// allow tests to reference Program
public partial class Program { }
