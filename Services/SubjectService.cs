using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

/// <summary>
/// Provides functionality for managing subjects, including
/// creation, retrieval, updates, and deletion for the current user.
/// </summary>
public class SubjectService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationStateProvider _authStateProvider;

    public SubjectService(
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

        // Ensure the user is authenticated
        if (principal.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("No authenticated user found.");

        var user = await _userManager.GetUserAsync(principal);

        // Ensure the user exists in the database
        if (user == null)
            throw new InvalidOperationException("Authenticated user could not be loaded.");

        return user;
    }

    /// <summary>
    /// Retrieves all subjects that belong to the current user,
    /// ordered alphabetically by name.
    /// </summary>
    public async Task<List<Subject>> GetSubjectsByUserAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .Where(s => s.UserId == user.Id)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all subjects for the current user,
    /// including their associated tasks.
    /// </summary>
    public async Task<List<Subject>> GetSubjectsWithTasksAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .Include(s => s.Tasks)
            .Where(s => s.UserId == user.Id)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific subject by ID for the current user,
    /// including its related tasks.
    /// Returns null if not found or not owned by the user.
    /// </summary>
    public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .Include(s => s.Tasks)
            .FirstOrDefaultAsync(s => s.Id == subjectId && s.UserId == user.Id);
    }

    /// <summary>
    /// Creates a new subject for the current user.
    /// </summary>
    public async Task<Subject> CreateSubjectAsync(Subject subject)
    {
        var user = await GetCurrentUserAsync();

        // Assign ownership and creation timestamp
        subject.UserId = user.Id;
        subject.CreatedAt = DateTime.UtcNow;

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return subject;
    }

    /// <summary>
    /// Updates an existing subject if it belongs to the current user.
    /// Returns false if the subject is not found or unauthorized.
    /// </summary>
    public async Task<bool> UpdateSubjectAsync(Subject updatedSubject)
    {
        var user = await GetCurrentUserAsync();

        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == updatedSubject.Id && s.UserId == user.Id);

        if (existingSubject == null)
            return false;

        // Update allowed fields
        existingSubject.Name = updatedSubject.Name;
        existingSubject.Description = updatedSubject.Description;
        existingSubject.Color = updatedSubject.Color;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a subject and its related tasks if it belongs to the current user.
    /// Returns false if not found or unauthorized.
    /// </summary>
    public async Task<bool> DeleteSubjectAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        var subject = await _context.Subjects
            .Include(s => s.Tasks)
            .FirstOrDefaultAsync(s => s.Id == subjectId && s.UserId == user.Id);

        if (subject == null)
            return false;

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Returns the total number of tasks associated with a subject for the current user.
    /// </summary>
    public async Task<int> GetTaskCountBySubjectAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .CountAsync(t => t.SubjectId == subjectId && t.UserId == user.Id);
    }

    /// <summary>
    /// Returns the number of completed tasks for a subject.
    /// </summary>
    public async Task<int> GetCompletedTaskCountBySubjectAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .CountAsync(t => t.SubjectId == subjectId && t.UserId == user.Id && t.IsCompleted);
    }

    /// <summary>
    /// Checks whether a subject belongs to the current user.
    /// Used for authorization validation.
    /// </summary>
    public async Task<bool> SubjectBelongsToCurrentUserAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .AnyAsync(s => s.Id == subjectId && s.UserId == user.Id);
    }
}