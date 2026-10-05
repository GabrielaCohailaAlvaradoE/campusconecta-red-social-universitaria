using System.ComponentModel.DataAnnotations;

namespace CampusConecta.Api.Models;

public sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(180)] string Email,
    [Required, RegularExpression("^[a-zA-Z0-9._]{3,30}$")] string Username,
    [Required, MinLength(2), MaxLength(80)] string DisplayName,
    [Required, MinLength(8), MaxLength(100)] string Password,
    [Required, RegularExpression("^(Student|Teacher|Staff)$")] string Role,
    [MaxLength(120)] string Faculty = "");

public sealed record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public sealed record AuthResponse(string Token, UserSummary User);
public sealed record UserSummary(Guid Id, string Username, string DisplayName, string Role, string Faculty, string AvatarUrl);
public sealed record UserProfile(Guid Id, string Email, string Username, string DisplayName, string Role, string Bio, string Faculty, string AvatarUrl, DateTimeOffset CreatedAt, int PostCount, int CommunityCount);
public sealed record UpdateProfileRequest([Required, MinLength(2), MaxLength(80)] string DisplayName, [MaxLength(320)] string Bio, [MaxLength(120)] string Faculty, [Url, MaxLength(500)] string? AvatarUrl);
public sealed record CreatePostRequest([Required, MinLength(1), MaxLength(1500)] string Content, [Url, MaxLength(500)] string? ImageUrl, Guid? CommunityId);
public sealed record CommentRequest([Required, MinLength(1), MaxLength(500)] string Content);
public sealed record ReactionRequest([Required, RegularExpression("^(Like|Celebrate|Support|Insightful)$")] string Type);
public sealed record CommentResponse(Guid Id, string Content, UserSummary Author, DateTimeOffset CreatedAt);
public sealed record PostResponse(Guid Id, string Content, string ImageUrl, UserSummary Author, Guid? CommunityId, string? CommunityName, DateTimeOffset CreatedAt, int CommentCount, Dictionary<string, int> Reactions, IReadOnlyList<CommentResponse> Comments);
public sealed record CreateCommunityRequest([Required, MinLength(3), MaxLength(80)] string Name, [Required, MinLength(10), MaxLength(500)] string Description);
public sealed record CommunityResponse(Guid Id, string Name, string Slug, string Description, UserSummary Owner, int MemberCount, bool IsMember, DateTimeOffset CreatedAt);
public sealed record SearchResponse(IReadOnlyList<UserSummary> Users, IReadOnlyList<PostResponse> Posts, IReadOnlyList<CommunityResponse> Communities);
public sealed record DashboardResponse(UserProfile Profile, int Posts, int Comments, int ReactionsReceived, int Communities, IReadOnlyList<PostResponse> RecentPosts);
