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
[Route("api/dashboard")]
public sealed class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        var userId = User.UserId();
        var user = await db.Users.AsNoTracking().SingleAsync(x => x.Id == userId);
        var postCount = await db.Posts.CountAsync(x => x.AuthorId == userId);
        var commentCount = await db.Comments.CountAsync(x => x.AuthorId == userId);
        var communityCount = await db.CommunityMembers.CountAsync(x => x.UserId == userId);
        var reactions = await db.Reactions.CountAsync(x => x.Post.AuthorId == userId);
        var recent = (await db.Posts.AsNoTracking().Where(x => x.AuthorId == userId).Include(x => x.Author).Include(x => x.Community).Include(x => x.Comments).ThenInclude(x => x.Author).Include(x => x.Reactions).ToListAsync())
            .OrderByDescending(x => x.CreatedAt).Take(5).ToList();
        var profile = new UserProfile(user.Id, user.Email, user.Username, user.DisplayName, user.Role, user.Bio, user.Faculty, user.AvatarUrl, user.CreatedAt, postCount, communityCount);
        return Ok(new DashboardResponse(profile, postCount, commentCount, reactions, communityCount, recent.Select(x => x.ToResponse()).ToList()));
    }
}
