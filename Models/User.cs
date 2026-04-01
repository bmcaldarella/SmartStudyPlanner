using Microsoft.AspNetCore.Identity;

namespace SmartStudyPlanner.Models;

/// <summary>
/// ApplicationUser extends IdentityUser with additional properties for the Study Planner app
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// User's full name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Date when the user created their account
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for user's tasks
    /// </summary>
    public ICollection<StudyTask> Tasks { get; set; } = new List<StudyTask>();

    /// <summary>
    /// Navigation property for user's subjects
    /// </summary>
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
