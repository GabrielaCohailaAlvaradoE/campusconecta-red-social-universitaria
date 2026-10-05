using System.Text;
using CampusConecta.Api.Data;
using CampusConecta.Api.Infrastructure;
using CampusConecta.Api.Models;
using CampusConecta.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var sqliteConnectionString = builder.Configuration.GetConnectionString("Sqlite");
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? "Host=localhost;Port=54329;Database=campusconecta;Username=campus;Password=campus_dev_password";
var jwtKey = builder.Configuration["Jwt:Key"] ?? "CampusConecta-Development-Key-Change-In-Production-2026";

if (!string.IsNullOrWhiteSpace(sqliteConnectionString))
{
    var sqlitePath = new SqliteConnectionStringBuilder(sqliteConnectionString).DataSource;
    var sqliteDirectory = Path.GetDirectoryName(sqlitePath);
    if (!string.IsNullOrWhiteSpace(sqliteDirectory))
        Directory.CreateDirectory(sqliteDirectory);
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(sqliteConnectionString))
        options.UseSqlite(sqliteConnectionString);
    else
        options.UseNpgsql(postgresConnectionString, postgres => postgres.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CampusConecta API", Version = "v1", Description = "API REST de la red social universitaria CampusConecta." });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CampusConecta",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CampusConecta.Web",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "CampusConecta.Api", timestamp = DateTimeOffset.UtcNow }));
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (string.IsNullOrWhiteSpace(sqliteConnectionString) && db.Database.IsRelational()) await db.Database.MigrateAsync(); else await db.Database.EnsureCreatedAsync();
    await DatabaseSeeder.SeedAsync(db);
}

app.Run();
public partial class Program { }
