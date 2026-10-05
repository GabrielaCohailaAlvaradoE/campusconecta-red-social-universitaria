namespace CampusConecta.Api.Models;

public sealed class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<Post> Posts { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public List<Reaction> Reactions { get; set; } = [];
    public List<CommunityMember> Memberships { get; set; } = [];
}

public sealed class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorId { get; set; }
    public Guid? CommunityId { get; set; }
    public required string Content { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public AppUser Author { get; set; } = null!;
    public Community? Community { get; set; }
    public List<Comment> Comments { get; set; } = [];
    public List<Reaction> Reactions { get; set; } = [];
}

public sealed class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public required string Content { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Post Post { get; set; } = null!;
    public AppUser Author { get; set; } = null!;
}

public sealed class Reaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public required string Type { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Post Post { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}

public sealed class Community
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public AppUser Owner { get; set; } = null!;
    public List<CommunityMember> Members { get; set; } = [];
    public List<Post> Posts { get; set; } = [];
}

public sealed class CommunityMember
{
    public Guid CommunityId { get; set; }
    public Guid UserId { get; set; }
    public required string Role { get; set; }
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    public Community Community { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
