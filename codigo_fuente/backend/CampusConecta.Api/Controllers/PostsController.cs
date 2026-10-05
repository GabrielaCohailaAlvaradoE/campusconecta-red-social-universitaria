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
[Route("api/posts")]
public sealed class PostsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PostResponse>>> Feed([FromQuery] Guid? communityId = null, [FromQuery] int limit = 30)
    {
        limit = Math.Clamp(limit, 1, 50);
        var query = FullPosts().AsNoTracking();
        if (communityId.HasValue) query = query.Where(x => x.CommunityId == communityId);
        // SQLite no admite ordenar DateTimeOffset en SQL; ordenar tras materializar
        // preserva el mismo comportamiento para el despliegue compacto.
        var posts = await query.ToListAsync();
        return Ok(posts.OrderByDescending(x => x.CreatedAt).Take(limit).Select(x => x.ToResponse()).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResponse>> Get(Guid id)
    {
        var post = await FullPosts().AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        return post is null ? NotFound() : Ok(post.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<PostResponse>> Create(CreatePostRequest request)
    {
        var userId = User.UserId();
        if (request.CommunityId.HasValue && !await db.CommunityMembers.AnyAsync(x => x.CommunityId == request.CommunityId && x.UserId == userId))
            return Forbid();
        var post = new Post { AuthorId = userId, Content = request.Content.Trim(), ImageUrl = request.ImageUrl?.Trim() ?? string.Empty, CommunityId = request.CommunityId };
        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = post.Id }, (await FullPosts().AsNoTracking().SingleAsync(x => x.Id == post.Id)).ToResponse());
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<CommentResponse>> Comment(Guid id, CommentRequest request)
    {
        if (!await db.Posts.AnyAsync(x => x.Id == id)) return NotFound();
        var comment = new Comment { PostId = id, AuthorId = User.UserId(), Content = request.Content.Trim() };
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        await db.Entry(comment).Reference(x => x.Author).LoadAsync();
        return StatusCode(201, new CommentResponse(comment.Id, comment.Content, comment.Author.ToSummary(), comment.CreatedAt));
    }

    [HttpPut("{id:guid}/reaction")]
    public async Task<ActionResult<PostResponse>> React(Guid id, ReactionRequest request)
    {
        if (!await db.Posts.AnyAsync(x => x.Id == id)) return NotFound();
        var userId = User.UserId();
        var reaction = await db.Reactions.SingleOrDefaultAsync(x => x.PostId == id && x.UserId == userId);
        if (reaction is null) db.Reactions.Add(new Reaction { PostId = id, UserId = userId, Type = request.Type });
        else reaction.Type = request.Type;
        await db.SaveChangesAsync();
        return Ok((await FullPosts().AsNoTracking().SingleAsync(x => x.Id == id)).ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var post = await db.Posts.SingleOrDefaultAsync(x => x.Id == id);
        if (post is null) return NotFound();
        if (post.AuthorId != User.UserId() && !User.IsInRole("Staff")) return Forbid();
        post.IsDeleted = true;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<Post> FullPosts() => db.Posts
        .Include(x => x.Author).Include(x => x.Community)
        .Include(x => x.Comments).ThenInclude(x => x.Author)
        .Include(x => x.Reactions);
}
