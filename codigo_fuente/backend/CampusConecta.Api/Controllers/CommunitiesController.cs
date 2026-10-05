using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
[Route("api/communities")]
public sealed class CommunitiesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CommunityResponse>>> List()
    {
        var userId = User.UserId();
        var items = await db.Communities.AsNoTracking().Where(x => x.IsActive).Include(x => x.Owner).Include(x => x.Members).OrderBy(x => x.Name).ToListAsync();
        return Ok(items.Select(x => Map(x, userId)).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CommunityResponse>> Get(Guid id)
    {
        var item = await db.Communities.AsNoTracking().Include(x => x.Owner).Include(x => x.Members).SingleOrDefaultAsync(x => x.Id == id && x.IsActive);
        return item is null ? NotFound() : Ok(Map(item, User.UserId()));
    }

    [HttpPost]
    public async Task<ActionResult<CommunityResponse>> Create(CreateCommunityRequest request)
    {
        var ownerId = User.UserId();
        var slugBase = Slugify(request.Name);
        var slug = slugBase;
        var suffix = 2;
        while (await db.Communities.AnyAsync(x => x.Slug == slug)) slug = $"{slugBase}-{suffix++}";
        var community = new Community { OwnerId = ownerId, Name = request.Name.Trim(), Slug = slug, Description = request.Description.Trim() };
        community.Members.Add(new CommunityMember { Community = community, CommunityId = community.Id, UserId = ownerId, Role = "Owner" });
        db.Communities.Add(community);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = community.Id }, Map(await db.Communities.AsNoTracking().Include(x => x.Owner).Include(x => x.Members).SingleAsync(x => x.Id == community.Id), ownerId));
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(Guid id)
    {
        var userId = User.UserId();
        if (!await db.Communities.AnyAsync(x => x.Id == id && x.IsActive)) return NotFound();
        if (!await db.CommunityMembers.AnyAsync(x => x.CommunityId == id && x.UserId == userId))
        {
            db.CommunityMembers.Add(new CommunityMember { CommunityId = id, UserId = userId, Role = "Member" });
            await db.SaveChangesAsync();
        }
        return NoContent();
    }

    [HttpDelete("{id:guid}/leave")]
    public async Task<IActionResult> Leave(Guid id)
    {
        var userId = User.UserId();
        var membership = await db.CommunityMembers.SingleOrDefaultAsync(x => x.CommunityId == id && x.UserId == userId);
        if (membership is null) return NotFound();
        if (membership.Role == "Owner") return Conflict(new ProblemDetails { Title = "La persona propietaria no puede abandonar la comunidad", Detail = "Transfiera la propiedad o elimine la comunidad.", Status = 409 });
        db.CommunityMembers.Remove(membership);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var community = await db.Communities.SingleOrDefaultAsync(x => x.Id == id);
        if (community is null) return NotFound();
        if (community.OwnerId != User.UserId() && !User.IsInRole("Staff")) return Forbid();
        community.IsActive = false;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static CommunityResponse Map(Community item, Guid userId) => new(item.Id, item.Name, item.Slug, item.Description, item.Owner.ToSummary(), item.Members.Count, item.Members.Any(x => x.UserId == userId), item.CreatedAt);

    private static string Slugify(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var withoutMarks = string.Concat(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        return Regex.Replace(withoutMarks, "[^a-z0-9]+", "-").Trim('-');
    }
}
