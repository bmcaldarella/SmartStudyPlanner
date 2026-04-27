using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

/// <summary>
/// Provides functionality for managing study tasks,
/// including creation, retrieval, updates, completion status,
/// and deletion for the current user.
/// </summary>
public class TaskService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationStateProvider _authStateProvider;

    public TaskService(
        ApplicationDbContext context,
        UserManager<User> userManager,
        AuthenticationStateProvider authStateProvider)
    {
        _context = context;
        _userManager = userManager;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Retrieves the currently authenticated user.
    /// Throws an exception if no user is authenticated.
    /// </summary>
    private async Task<User> GetCurrentUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;

        if (principal.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("No authenticated user found.");

        var user = await _userManager.GetUserAsync(principal);

        if (user == null)
            throw new InvalidOperationException("Authenticated user could not be loaded.");

        return user;
    }

    /// <summary>
    /// Retrieves all tasks for the current user,
    /// ordered by completion status, deadline, and priority.
    /// </summary>
    public async Task<List<StudyTask>> GetTasksByUserAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .Include(t => t.Subject)
            .Where(t => t.UserId == user.Id)
            .OrderBy(t => t.IsCompleted) // incomplete tasks first
            .ThenBy(t => t.Deadline)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all tasks for a specific subject belonging to the current user.
    /// </summary>
    public async Task<List<StudyTask>> GetTasksBySubjectAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .Include(t => t.Subject)
            .Where(t => t.UserId == user.Id && t.SubjectId == subjectId)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Deadline)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a single task by ID for the current user.
    /// Returns null if not found or not owned by the user.
    /// </summary>
    public async Task<StudyTask?> GetTaskByIdAsync(int taskId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);
    }

    /// <summary>
    /// Retrieves all completed tasks for the current user,
    /// ordered by completion date (most recent first).
    /// </summary>
    public async Task<List<StudyTask>> GetCompletedTasksAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .Include(t => t.Subject)
            .Where(t => t.UserId == user.Id && t.IsCompleted)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all pending (not completed) tasks for the current user,
    /// ordered by deadline and priority.
    /// </summary>
    public async Task<List<StudyTask>> GetPendingTasksAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .Include(t => t.Subject)
            .Where(t => t.UserId == user.Id && !t.IsCompleted)
            .OrderBy(t => t.Deadline)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves upcoming tasks within a specified number of days.
    /// Default is 7 days.
    /// </summary>
    public async Task<List<StudyTask>> GetUpcomingTasksAsync(int days = 7)
    {
        var user = await GetCurrentUserAsync();
        var today = DateTime.UtcNow;
        var futureDate = today.AddDays(days);

        return await _context.StudyTasks
            .Include(t => t.Subject)
            .Where(t =>
                t.UserId == user.Id &&
                !t.IsCompleted &&
                t.Deadline.HasValue &&
                t.Deadline.Value >= today &&
                t.Deadline.Value <= futureDate)
            .OrderBy(t => t.Deadline)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new task for the current user.
    /// Validates that the selected subject belongs to the user.
    /// </summary>
    public async Task<StudyTask> CreateTaskAsync(StudyTask task)
    {
        var user = await GetCurrentUserAsync();

        // Ensure subject belongs to the current user
        var subjectExists = await _context.Subjects
            .AnyAsync(s => s.Id == task.SubjectId && s.UserId == user.Id);

        if (!subjectExists)
            throw new InvalidOperationException("The selected subject does not exist or does not belong to the current user.");

        task.UserId = user.Id;
        task.CreatedAt = DateTime.UtcNow;

        // Automatically set completion timestamp if already completed
        if (task.IsCompleted && !task.CompletedAt.HasValue)
            task.CompletedAt = DateTime.UtcNow;

        _context.StudyTasks.Add(task);
        await _context.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// Updates an existing task if it belongs to the current user.
    /// Also manages completion state transitions.
    /// </summary>
    public async Task<bool> UpdateTaskAsync(StudyTask updatedTask)
    {
        var user = await GetCurrentUserAsync();

        var existingTask = await _context.StudyTasks
            .FirstOrDefaultAsync(t => t.Id == updatedTask.Id && t.UserId == user.Id);

        if (existingTask == null)
            return false;

        // Ensure subject belongs to the current user
        var subjectExists = await _context.Subjects
            .AnyAsync(s => s.Id == updatedTask.SubjectId && s.UserId == user.Id);

        if (!subjectExists)
            throw new InvalidOperationException("The selected subject does not exist or does not belong to the current user.");

        // Update editable fields
        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.Deadline = updatedTask.Deadline;
        existingTask.SubjectId = updatedTask.SubjectId;
        existingTask.Priority = updatedTask.Priority;

        // Handle completion state changes
        if (!existingTask.IsCompleted && updatedTask.IsCompleted)
        {
            existingTask.IsCompleted = true;
            existingTask.CompletedAt = DateTime.UtcNow;
        }
        else if (existingTask.IsCompleted && !updatedTask.IsCompleted)
        {
            existingTask.IsCompleted = false;
            existingTask.CompletedAt = null;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Marks a task as completed and sets the completion timestamp.
    /// </summary>
    public async Task<bool> MarkTaskAsCompletedAsync(int taskId)
    {
        var user = await GetCurrentUserAsync();

        var task = await _context.StudyTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);

        if (task == null)
            return false;

        task.IsCompleted = true;
        task.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Marks a task as pending (not completed) and clears the completion timestamp.
    /// </summary>
    public async Task<bool> MarkTaskAsPendingAsync(int taskId)
    {
        var user = await GetCurrentUserAsync();

        var task = await _context.StudyTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);

        if (task == null)
            return false;

        task.IsCompleted = false;
        task.CompletedAt = null;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a task if it belongs to the current user.
    /// </summary>
    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var user = await GetCurrentUserAsync();

        var task = await _context.StudyTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == user.Id);

        if (task == null)
            return false;

        _context.StudyTasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}