namespace Application.Requests.Identity.User;

public class RefreshTokenRequest
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}