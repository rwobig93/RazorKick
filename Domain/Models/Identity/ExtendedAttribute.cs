using Shared.Enums.Identity;

namespace Domain.Models.Identity;

public class ExtendedAttribute
{
    private string _value = "";

    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; init; } = "";
    public string Value
    {
        get => _value;
        set {
            PreviousValue = _value;
            _value = value;
            Updated = DateTime.Now;
        }
    }
    public string PreviousValue { get; private set; } = "";
    public AttributeType Type { get; init; }
    public DateTime Created { get; } = DateTime.Now;
    public DateTime Updated { get; private set; }
}