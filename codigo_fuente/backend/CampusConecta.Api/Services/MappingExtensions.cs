using CampusConecta.Api.Models;

namespace CampusConecta.Api.Services;

public static class MappingExtensions
{
    public static UserSummary ToSummary(this AppUser user) => new(user.Id, user.Username, user.DisplayName, user.Role, user.Faculty, user.AvatarUrl);

    public static PostResponse ToResponse(this Post post) => new(
        post.Id,
        post.Content,
        post.ImageUrl,
        post.Author.ToSummary(),
        post.CommunityId,
        post.Community?.Name,
        post.CreatedAt,
        post.Comments.Count,
        post.Reactions.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Count()),
        post.Comments.OrderBy(x => x.CreatedAt).Select(x => new CommentResponse(x.Id, x.Content, x.Author.ToSummary(), x.CreatedAt)).ToList());
}
