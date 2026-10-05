using CampusConecta.Api.Data;
using CampusConecta.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConecta.Api.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;
        var hasher = new PasswordHasher<AppUser>();
        var gabriela = NewUser("gabriela@campus.test", "gabriela", "Gabriela Cohaila", "Student", "Ingeniería");
        var victoria = NewUser("victoria@campus.test", "victoria", "Victoria Lavarello", "Student", "Ingeniería");
        var docente = NewUser("docente@campus.test", "docente", "Marco Salazar", "Teacher", "Ciencias de la Computación");
        foreach (var user in new[] { gabriela, victoria, docente }) user.PasswordHash = hasher.HashPassword(user, "Demo123!");

        var community = new Community { Name = "Innovación Universitaria", Slug = "innovacion-universitaria", Description = "Comunidad para compartir proyectos, convocatorias y aprendizajes del campus.", OwnerId = docente.Id, Owner = docente };
        community.Members.Add(new CommunityMember { CommunityId = community.Id, UserId = docente.Id, Community = community, User = docente, Role = "Owner" });
        community.Members.Add(new CommunityMember { CommunityId = community.Id, UserId = gabriela.Id, Community = community, User = gabriela, Role = "Member" });
        community.Members.Add(new CommunityMember { CommunityId = community.Id, UserId = victoria.Id, Community = community, User = victoria, Role = "Member" });

        var post1 = new Post { Author = docente, AuthorId = docente.Id, Community = community, CommunityId = community.Id, Content = "Bienvenidos a CampusConecta. Este espacio reúne iniciativas, eventos y colaboración académica." };
        var post2 = new Post { Author = gabriela, AuthorId = gabriela.Id, Content = "Comparto una guía breve para organizar equipos de proyecto y mantener visibles los avances." };
        post1.Comments.Add(new Comment { Author = victoria, AuthorId = victoria.Id, Post = post1, PostId = post1.Id, Content = "Excelente iniciativa. Me uno para compartir las próximas convocatorias." });
        post1.Reactions.Add(new Reaction { User = gabriela, UserId = gabriela.Id, Post = post1, PostId = post1.Id, Type = "Celebrate" });
        post2.Reactions.Add(new Reaction { User = victoria, UserId = victoria.Id, Post = post2, PostId = post2.Id, Type = "Like" });

        db.AddRange(gabriela, victoria, docente, community, post1, post2);
        await db.SaveChangesAsync();
    }

    private static AppUser NewUser(string email, string username, string name, string role, string faculty) => new()
    {
        Email = email,
        Username = username,
        DisplayName = name,
        PasswordHash = string.Empty,
        Role = role,
        Faculty = faculty,
        Bio = $"Miembro de la comunidad universitaria en {faculty}."
    };
}
