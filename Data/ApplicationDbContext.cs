using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Data;

/// <summary>
/// Application database context for Entity Framework Core
/// Inherits from IdentityDbContext to support ASP.NET Core Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for Study Tasks
    /// </summary>
    public DbSet<StudyTask> StudyTasks { get; set; } = null!;

    /// <summary>
    /// DbSet for Subjects
    /// </summary>
    public DbSet<Subject> Subjects { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure StudyTask relationships
        builder.Entity<StudyTask>()
            .HasOne(t => t.Subject)
            .WithMany(s => s.Tasks)
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StudyTask>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Subject relationships
        builder.Entity<Subject>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subjects)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add indexes for better query performance
        builder.Entity<StudyTask>()
            .HasIndex(t => t.UserId);

        builder.Entity<StudyTask>()
            .HasIndex(t => t.SubjectId);

        builder.Entity<Subject>()
            .HasIndex(s => s.UserId);
    }
}
