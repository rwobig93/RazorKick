namespace Shared.Requests.Example;

public class ExampleObjectUpdateRequest
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}