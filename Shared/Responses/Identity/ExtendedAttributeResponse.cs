using Shared.Enums.Identity;

namespace Shared.Responses.Identity;

public class ExtendedAttributeResponse
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExtendedAttributeType Type { get; set; }
}