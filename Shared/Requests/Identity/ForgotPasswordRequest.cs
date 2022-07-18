using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Identity;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}