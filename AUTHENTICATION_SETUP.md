# Authentication System - What I've Set Up

## Hey Team! Here's What I've Done

I've built out a complete authentication system using **ASP.NET Core Identity** with a custom `User` model. This gives us secure account management with password hashing, lockout policies, and user claims right out of the box. You can now register, log in, and log out.

## Key Components (What I've added)

### 1. Models (in `Models/` folder)
- **User.cs** - I extended IdentityUser with FirstName, LastName, CreatedAt, and linked it to Tasks and Subjects
- **Subject.cs** - Represents a study course/subject that users can create
- **StudyTask.cs** - Represents tasks/assignments within a subject
- **InputModels/** - I separated the form validation into dedicated files (RegisterInput.cs, LoginInput.cs) for clarity

### 2. Database Context (`Data/ApplicationDbContext.cs`)
- I set it up to inherit from `IdentityDbContext<User>` so we automatically get all the required (built-in) Identity tables
- Added proper relationships and cascade deletes (deleting a user removes all their tasks/subjects)
- I added indexes on UserId and SubjectId for better query performance

### 3. Authentication Pages (`Components/Pages/Account/`)
- **Register.razor** - Fully working registration page with validation
- **Login.razor** - Fully working login page that authenticates users
- Both use `[SupplyParameterFromForm]` for static SSR form binding in .NET 8

### 4. I Configured Everything in `Program.cs`
```csharp
// What I set up:
- AddDbContext<ApplicationDbContext>() with SQLite
- AddIdentity<User, IdentityRole>() with configured password rules
- AddCascadingAuthenticationState() so auth state is global (available) everywhere
- app.UseAuthentication() and app.UseAuthorization()
- db.Database.Migrate() on startup for auto-migrations
```

### Password Requirements (Dev Environment)

To make testing easier for us right now, I have significantly relaxed the password rules. Passwords currently only require:

- A minimum of 6 characters.
- No special characters, no required digits, and no uppercase/lowercase requirements.

*(I will harden these in `Program.cs` before we push to production).*

Want to change this? Just edit `Program.cs` lines 23-30 in the Identity configuration and customize away.

## Database - How I Set It Up
I'm using SQLite with auto-migrations via `db.Database.Migrate()` in `Program.cs. When the app starts, it automatically applies any pending migrations, so you don't have to worry about running migration commands locally.

If you make changes to my database models, you need to generate a new migration. Run this in your terminal:

```bash
cd SmartStudyPlanner
dotnet ef migrations add <DescribeYourChangeHere>

This will create a new file in the Migrations/ folder. The database will automatically apply it the next time you run the app.


## How You Can Use Authentication in Your Components

1. Getting the Current User in a Service (Backend)
CRITICAL: Do not use IHttpContextAccessor in Blazor 8 services, as it will crash during interactive rendering! Please inject the AuthenticationStateProvider instead, like this:

C#
public class MyService
{
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationStateProvider _authStateProvider;

    public MyService(UserManager<User> userManager, AuthenticationStateProvider authStateProvider)
    {
        _userManager = userManager;
        _authStateProvider = authStateProvider;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var username = authState.User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
            return null; // No one is logged in
            
        return await _userManager.FindByNameAsync(username);
    }
}
2. Checking the User in a UI Component (Frontend)
If you just need to show or hide HTML based on whether someone is logged in, use the context provided by the cascading auth state:

Razor CSHTML
@if (context.User.Identity?.IsAuthenticated ?? false)
{
    <p>Welcome, @context.User.Identity.Name</p>
}
else
{
    <p>Please <a href="/Account/Login">log in</a> to see your dashboard.</p>
}
```

## Architectural Decisions & Notes

### Cascade Deletes
When a user is deleted, all their Tasks and Subjects are automatically deleted. This keeps our dev database clean, but we will likely change this to a "soft delete" or archive system before launch.

### Antiforgery is Disabled
Security is enabled by default in our .NET 8 Blazor setup. Whenever you build a static form, make sure you use the <EditForm> component, and Blazor will automatically generate and handle the valid Antiforgery tokens for you securely.

### Static SSR Pages
The Register/Login pages use static server-side rendering (no `@rendermode InteractiveServer`). This is intentional so authentication context works properly.

## Common Tasks When Building Features

### Getting the Current User in a Service
If you're building a service (like TaskService or SubjectService), here's how to get the current user:

```csharp
public class MyService
{
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationStateProvider _authStateProvider;

    public MyService(UserManager<User> userManager, AuthenticationStateProvider authStateProvider)
    {
        _userManager = userManager;
        _authStateProvider = authStateProvider;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var username = authState.User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
            return null;
            
        return await _userManager.FindByNameAsync(username);
    }
}
```

### Checking Authentication in a Component
```razor
@if (context.User.Identity?.IsAuthenticated ?? false)
{
    <p>Welcome, @context.User.Identity.Name</p>
}
else
{
    <p>Please <a href="/Account/Login">log in</a></p>
}
```

## What's Next - What I Need You All to Do

**Here's what's left:**

1. **Me (Efehi)**: I'm building the Services/Repository layer next
   - TaskService, SubjectService, UserService
   - CRUD operations using this auth foundation
   - Dependency injection setup in Program.cs

2. **Camila**: You can start building the `/planner` dashboard
   - Use the services I'll provide
   - Display user's subjects and tasks
   - Create/edit/delete functionality

3. **Stanford**: Polish the UI/CSS for Register and Login pages
   - I know they're functional but pretty basic right now
   - Maybe we can make them look good!

## Quick Troubleshooting

- **"User not found"**: Check that `UserManager.CreateAsync(user, password)` actually succeeded
- **Auth state not showing**: Make sure `CascadingAuthenticationState` is registered in the service chain
- **Password validation failing**: Look at the Identity configuration in Program.cs lines 23-30
