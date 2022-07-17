namespace Shared.Requests.Identity;

public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public bool? IsActive { get; set; }
}