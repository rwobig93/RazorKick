namespace Application.Requests.Identity.User;

public class RefreshTokenRequest
{
    public string ClientId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}