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
[Route("api/search")]
public sealed class SearchController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SearchResponse>> Search([FromQuery] string q)
    {
        q = q?.Trim() ?? string.Empty;
        if (q.Length < 2) return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { ["q"] = ["Ingrese al menos dos caracteres."] }));
        var term = q.ToLower();
        var users = await db.Users.AsNoTracking().Where(x => x.IsActive && (x.DisplayName.ToLower().Contains(term) || x.Username.ToLower().Contains(term))).OrderBy(x => x.DisplayName).Take(10).ToListAsync();
        var posts = (await db.Posts.AsNoTracking().Where(x => x.Content.ToLower().Contains(term)).Include(x => x.Author).Include(x => x.Community).Include(x => x.Comments).ThenInclude(x => x.Author).Include(x => x.Reactions).ToListAsync())
            .OrderByDescending(x => x.CreatedAt).Take(10).ToList();
        var communities = await db.Communities.AsNoTracking().Where(x => x.IsActive && (x.Name.ToLower().Contains(term) || x.Description.ToLower().Contains(term))).Include(x => x.Owner).Include(x => x.Members).OrderBy(x => x.Name).Take(10).ToListAsync();
        var userId = User.UserId();
        return Ok(new SearchResponse(
            users.Select(x => x.ToSummary()).ToList(),
            posts.Select(x => x.ToResponse()).ToList(),
            communities.Select(x => new CommunityResponse(x.Id, x.Name, x.Slug, x.Description, x.Owner.ToSummary(), x.Members.Count, x.Members.Any(m => m.UserId == userId), x.CreatedAt)).ToList()));
    }
}
