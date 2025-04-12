using CvAPI2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class CvDbContext : IdentityDbContext<User, Role, string>
{
    public CvDbContext(DbContextOptions<CvDbContext> options) : base(options)
        {}
    public DbSet<Cv> Cvs { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Education> Educations { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One-to-one: User <-> Cv
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cv)
            .WithOne(c => c.User)
            .HasForeignKey<Cv>(c => c.UserId);

        // One-to-many: Role -> Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);

        // One-to-many: Cv -> WorkExperiences
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.WorkExperiences)
            .WithOne(w => w.Cv)
            .HasForeignKey(w => w.CvId);

        // One-to-many: Cv -> Educations
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.Educations)
            .WithOne(e => e.Cv)
            .HasForeignKey(e => e.CvId);
    }
}