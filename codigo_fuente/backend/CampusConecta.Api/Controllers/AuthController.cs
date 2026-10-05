using System.Text.RegularExpressions;
using CampusConecta.Api.Data;
using CampusConecta.Api.Models;
using CampusConecta.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConecta.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext db, IPasswordHasher<AppUser> hasher, JwtService jwt) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim().ToLowerInvariant();
        if (!Regex.IsMatch(request.Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$"))
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { ["password"] = ["La contraseña debe incluir mayúscula, minúscula y número."] }));
        if (await db.Users.AnyAsync(x => x.Email == email || x.Username == username))
            return Conflict(new ProblemDetails { Title = "Cuenta existente", Detail = "El correo o nombre de usuario ya está registrado.", Status = 409 });

        var user = new AppUser
        {
            Email = email,
            Username = username,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = string.Empty,
            Role = request.Role,
            Faculty = request.Faculty.Trim()
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created, new AuthResponse(jwt.CreateToken(user), user.ToSummary()));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email && x.IsActive);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new ProblemDetails { Title = "Credenciales inválidas", Detail = "El correo o la contraseña no son correctos.", Status = 401 });
        return Ok(new AuthResponse(jwt.CreateToken(user), user.ToSummary()));
    }
}
