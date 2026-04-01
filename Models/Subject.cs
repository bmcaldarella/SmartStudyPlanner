namespace SmartStudyPlanner.Models;

/// <summary>
/// Represents a subject/course that a user is studying
/// </summary>
public class Subject
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the subject (e.g., "Mathematics", "History")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the subject
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Color code for UI display (e.g., "#FF5733")
    /// </summary>
    public string? Color { get; set; } = "#0d6efd";

    /// <summary>
    /// Date when the subject was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who owns this subject
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user who owns this subject
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Navigation property for tasks in this subject
    /// </summary>
    public ICollection<StudyTask> Tasks { get; set; } = new List<StudyTask>();
}
