using CampusConecta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusConecta.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Community> Communities => Set<Community>();
    public DbSet<CommunityMember> CommunityMembers => Set<CommunityMember>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(180);
            entity.Property(x => x.Username).HasMaxLength(30);
            entity.Property(x => x.DisplayName).HasMaxLength(80);
            entity.Property(x => x.Role).HasMaxLength(20);
        });

        builder.Entity<Post>(entity =>
        {
            entity.Property(x => x.Content).HasMaxLength(1500);
            entity.HasQueryFilter(x => !x.IsDeleted);
            entity.HasOne(x => x.Author).WithMany(x => x.Posts).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Community).WithMany(x => x.Posts).HasForeignKey(x => x.CommunityId).OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Comment>(entity =>
        {
            entity.Property(x => x.Content).HasMaxLength(500);
            entity.HasQueryFilter(x => !x.IsDeleted);
            entity.HasOne(x => x.Author).WithMany(x => x.Comments).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Post).WithMany(x => x.Comments).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Reaction>(entity =>
        {
            entity.HasIndex(x => new { x.PostId, x.UserId }).IsUnique();
            entity.HasQueryFilter(x => !x.Post.IsDeleted);
            entity.HasOne(x => x.User).WithMany(x => x.Reactions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Post).WithMany(x => x.Reactions).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Community>(entity =>
        {
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(80);
            entity.Property(x => x.Slug).HasMaxLength(90);
            entity.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CommunityMember>(entity =>
        {
            entity.HasKey(x => new { x.CommunityId, x.UserId });
            entity.HasOne(x => x.Community).WithMany(x => x.Members).HasForeignKey(x => x.CommunityId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.User).WithMany(x => x.Memberships).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
