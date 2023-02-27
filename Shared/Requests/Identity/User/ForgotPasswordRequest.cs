using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Identity.User;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}