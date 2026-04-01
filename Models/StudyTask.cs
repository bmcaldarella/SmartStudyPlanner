namespace SmartStudyPlanner.Models;

/// <summary>
/// Represents a study task that a user needs to complete
/// </summary>
public class StudyTask
{
    public int Id { get; set; }

    /// <summary>
    /// Title of the task
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Deadline for completing the task
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// Whether the task has been completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Date when the task was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the task was marked as completed (if applicable)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Subject ID this task belongs to
    /// </summary>
    public int SubjectId { get; set; }

    /// <summary>
    /// Navigation property to the subject
    /// </summary>
    public Subject? Subject { get; set; }

    /// <summary>
    /// User ID who owns this task
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Priority level (1=Low, 2=Medium, 3=High)
    /// </summary>
    public int Priority { get; set; } = 2;
}
