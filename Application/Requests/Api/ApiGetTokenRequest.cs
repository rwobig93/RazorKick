namespace Application.Requests.Api;

public class ApiGetTokenRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}