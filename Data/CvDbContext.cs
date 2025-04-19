using CvAPI2.Models;
using CvAPI2.Models.Tag;
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
    public DbSet<Award> Awards { get; set; }
    public DbSet<Certification> Certifications { get; set; }
    public DbSet<CompetenceOverview> CompetenceOverviews { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<ProjectExperience> ProjectExperiences { get; set; }
    public DbSet<RoleOverview> RoleOverviews { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<WorkExperienceTag> WorkExperienceTags { get; set; }
    public DbSet<ProjectExperienceTag> ProjectExperienceTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One-to-one: User <-> Cv
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cv)
            .WithOne(c => c.User)
            .HasForeignKey<Cv>(c => c.UserId);

        // Gjør UserId unik i Cv-tabellen
        modelBuilder.Entity<Cv>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        // One-to-many: Cv -> WorkExperiences....
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.WorkExperiences)
            .WithOne(w => w.Cv)
            .HasForeignKey(w => w.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.Educations)
            .WithOne(e => e.Cv)
            .HasForeignKey(e => e.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.Awards)
            .WithOne(a => a.Cv)
            .HasForeignKey(a => a.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.Certifications)
            .WithOne(c => c.Cv)
            .HasForeignKey(c => c.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.CompetenceOverviews)
            .WithOne(com => com.Cv)
            .HasForeignKey(com => com.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.Courses)
            .WithOne(cor => cor.Cv)
            .HasForeignKey(cor => cor.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.Languages)
            .WithOne(l => l.Cv)
            .HasForeignKey(l => l.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.ProjectExperiences)
            .WithOne(p => p.Cv)
            .HasForeignKey(p => p.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cv>()
            .HasMany(c => c.RoleOverviews)
            .WithOne(r => r.Cv)
            .HasForeignKey(r => r.CvId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkExperienceTag>()
            .HasKey(wet => new { wet.WorkExperienceId, wet.TagId });

        modelBuilder.Entity<WorkExperienceTag>()
            .HasOne(wet => wet.WorkExperience)
            .WithMany(w => w.Tags)
            .HasForeignKey(wet => wet.WorkExperienceId);   
                 
        modelBuilder.Entity<WorkExperienceTag>()
            .HasOne(wet => wet.Tag)
            .WithMany()
            .HasForeignKey(wet => wet.TagId);
        
        modelBuilder.Entity<ProjectExperienceTag>()
            .HasKey(pet => new { pet.ProjectExperienceId, pet.TagId });

        modelBuilder.Entity<ProjectExperienceTag>()
            .HasOne(pet => pet.ProjectExperience)
            .WithMany(p => p.Tags)
            .HasForeignKey(pet => pet.ProjectExperienceId);    

        modelBuilder.Entity<ProjectExperienceTag>()
            .HasOne(pet => pet.Tag)
            .WithMany()
            .HasForeignKey(pet => pet.TagId);        
    }

}