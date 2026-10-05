using CampusConecta.Api.Data;
using CampusConecta.Api.Infrastructure;
using CampusConecta.Api.Models;
using CampusConecta.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConecta.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserProfile>> Me() => await FindProfile(User.UserId()) is { } profile ? Ok(profile) : NotFound();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfile>> Get(Guid id) => await FindProfile(id) is { } profile ? Ok(profile) : NotFound();

    [HttpPut("me")]
    public async Task<ActionResult<UserProfile>> Update(UpdateProfileRequest request)
    {
        var user = await db.Users.FindAsync(User.UserId());
        if (user is null) return NotFound();
        user.DisplayName = request.DisplayName.Trim();
        user.Bio = request.Bio.Trim();
        user.Faculty = request.Faculty.Trim();
        user.AvatarUrl = request.AvatarUrl?.Trim() ?? string.Empty;
        await db.SaveChangesAsync();
        return Ok(await FindProfile(user.Id));
    }

    private Task<UserProfile?> FindProfile(Guid id) => db.Users.AsNoTracking().Where(x => x.Id == id && x.IsActive)
        .Select(x => new UserProfile(x.Id, x.Email, x.Username, x.DisplayName, x.Role, x.Bio, x.Faculty, x.AvatarUrl, x.CreatedAt, x.Posts.Count, x.Memberships.Count))
        .SingleOrDefaultAsync();
}
