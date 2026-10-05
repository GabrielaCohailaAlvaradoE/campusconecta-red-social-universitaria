using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CampusConecta.Api.Models;
using CampusConecta.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CampusConecta.Tests;

public sealed class ApiFlowTests(CampusApiFactory factory) : IClassFixture<CampusApiFactory>
{
    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await factory.CreateClient().GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SeededAccount_HasValidPassword()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.SingleAsync(x => x.Email == "gabriela@campus.test");
        var result = new PasswordHasher<AppUser>().VerifyHashedPassword(user, user.PasswordHash, "Demo123!");
        Assert.NotEqual(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public async Task Register_RejectsWeakPassword_AndAcceptsValidUser()
    {
        var client = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var weak = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest($"weak{suffix}@campus.test", $"weak{suffix}", "Usuario Prueba", "password", "Student", "Ingeniería"));
        Assert.Equal(HttpStatusCode.BadRequest, weak.StatusCode);

        var valid = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest($"valid{suffix}@campus.test", $"valid{suffix}", "Usuario Prueba", "Secure123!", "Student", "Ingeniería"));
        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
        var auth = await valid.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(auth?.Token));
    }

    [Fact]
    public async Task Login_Feed_Post_Comment_Reaction_AndDashboard_Work()
    {
        var client = factory.CreateClient();
        var auth = await Login(client, "gabriela@campus.test", "Demo123!");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var feed = await client.GetFromJsonAsync<List<PostResponse>>("/api/posts");
        Assert.NotNull(feed);
        Assert.NotEmpty(feed);

        var create = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest("Publicación creada durante una prueba automática de integración.", null, null));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var post = await create.Content.ReadFromJsonAsync<PostResponse>();
        Assert.NotNull(post);

        var comment = await client.PostAsJsonAsync($"/api/posts/{post.Id}/comments", new CommentRequest("Comentario verificado por la prueba."));
        Assert.Equal(HttpStatusCode.Created, comment.StatusCode);
        var reaction = await client.PutAsJsonAsync($"/api/posts/{post.Id}/reaction", new ReactionRequest("Insightful"));
        Assert.Equal(HttpStatusCode.OK, reaction.StatusCode);

        var dashboard = await client.GetFromJsonAsync<DashboardResponse>("/api/dashboard");
        Assert.NotNull(dashboard);
        Assert.True(dashboard.Posts >= 2);
        Assert.True(dashboard.Comments >= 1);
    }

    [Fact]
    public async Task Communities_Search_Profile_AndMembership_Work()
    {
        var client = factory.CreateClient();
        var auth = await Login(client, "victoria@campus.test", "Demo123!");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var createdResponse = await client.PostAsJsonAsync("/api/communities", new CreateCommunityRequest("Calidad de Software", "Espacio para compartir estrategias y resultados de pruebas de software."));
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var community = await createdResponse.Content.ReadFromJsonAsync<CommunityResponse>();
        Assert.NotNull(community);
        Assert.True(community.IsMember);

        var search = await client.GetFromJsonAsync<SearchResponse>("/api/search?q=Calidad");
        Assert.NotNull(search);
        Assert.Contains(search.Communities, x => x.Id == community.Id);

        var profile = await client.PutAsJsonAsync("/api/users/me", new UpdateProfileRequest("Victoria Lavarello", "Interesada en pruebas y experiencia de usuario.", "Ingeniería", null));
        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_RequiresJwt()
    {
        var response = await factory.CreateClient().GetAsync("/api/dashboard");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<AuthResponse> Login(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }
}
