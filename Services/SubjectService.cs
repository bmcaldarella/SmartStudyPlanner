using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartStudyPlanner.Data;
using SmartStudyPlanner.Models;

namespace SmartStudyPlanner.Services;

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

    public async Task<List<Subject>> GetSubjectsByUserAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .Where(s => s.UserId == user.Id)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<Subject>> GetSubjectsWithTasksAsync()
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .Include(s => s.Tasks)
            .Where(s => s.UserId == user.Id)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .Include(s => s.Tasks)
            .FirstOrDefaultAsync(s => s.Id == subjectId && s.UserId == user.Id);
    }

    public async Task<Subject> CreateSubjectAsync(Subject subject)
    {
        var user = await GetCurrentUserAsync();

        subject.UserId = user.Id;
        subject.CreatedAt = DateTime.UtcNow;

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return subject;
    }

    public async Task<bool> UpdateSubjectAsync(Subject updatedSubject)
    {
        var user = await GetCurrentUserAsync();

        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == updatedSubject.Id && s.UserId == user.Id);

        if (existingSubject == null)
            return false;

        existingSubject.Name = updatedSubject.Name;
        existingSubject.Description = updatedSubject.Description;
        existingSubject.Color = updatedSubject.Color;

        await _context.SaveChangesAsync();
        return true;
    }

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

    public async Task<int> GetTaskCountBySubjectAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .CountAsync(t => t.SubjectId == subjectId && t.UserId == user.Id);
    }

    public async Task<int> GetCompletedTaskCountBySubjectAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.StudyTasks
            .CountAsync(t => t.SubjectId == subjectId && t.UserId == user.Id && t.IsCompleted);
    }

    public async Task<bool> SubjectBelongsToCurrentUserAsync(int subjectId)
    {
        var user = await GetCurrentUserAsync();

        return await _context.Subjects
            .AnyAsync(s => s.Id == subjectId && s.UserId == user.Id);
    }
}