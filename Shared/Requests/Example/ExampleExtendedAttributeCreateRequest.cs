using Shared.Enums.Example;

namespace Shared.Requests.Example;

public class ExampleExtendedAttributeCreateRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExampleExtendedAttributeType Type { get; set; }
}