namespace Application.Services.Identity;

public interface ICurrentUserService
{
    string UserId { get; }
    public List<KeyValuePair<string, string>> Claims { get; }
}