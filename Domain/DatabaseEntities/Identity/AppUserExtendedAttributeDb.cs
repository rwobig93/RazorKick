using Domain.Enums.Identity;
using Shared.Responses.Identity;

namespace Domain.DatabaseEntities.Identity;

public class AppUserExtendedAttributeDb
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public ExtendedAttributeType Type { get; set; }
}

public static class AppUserExtendedAttributeDbExtensions
{
    public static ExtendedAttributeResponse ToResponse(this AppUserExtendedAttributeDb attribute)
    {
        return new ExtendedAttributeResponse
        {
            Id = attribute.Id,
            OwnerId = attribute.OwnerId,
            Name = attribute.Name,
            Value = attribute.Value,
            Type = (Shared.Enums.Identity.ExtendedAttributeType) attribute.Type
        };
    }

    public static List<ExtendedAttributeResponse> ToResponses(this IEnumerable<AppUserExtendedAttributeDb> attributes)
    {
        return attributes.Select(x => x.ToResponse()).ToList();
    }
}