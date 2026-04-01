using System.ComponentModel.DataAnnotations;

namespace SmartStudyPlanner.Models.InputModels;

/// <summary>
/// Input model for user login form
/// Used by Login.razor to capture and validate credentials
/// </summary>
public class LoginInput
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
