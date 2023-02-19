namespace Shared.Responses.Example;

public class ExampleObjectFullResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public List<ExampleExtendedAttributeResponse> ExtendedAttributes { get; set; } = new();
    public List<ExamplePermissionResponse> Permissions { get; set; } = new();
}