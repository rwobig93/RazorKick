using Shared.Enums.Identity;

namespace Domain.Models.Identity;

public class ExtendedAttribute
{
    private DateTime _updated = DateTime.Now;
    private string _value = "";
    private List<string> _previousValues = new List<string>();

    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; init; } = "";
    public string Value => _value;
    public bool Completed { get; set; } = false;
    public List<string> PreviousValues => _previousValues;
    public AttributeType Type { get; init; }
    public DateTime Added { get; } = DateTime.Now;
    public DateTime Updated => _updated;
    
    public void Update(string newValue)
    {
        _updated = DateTime.Now;
        if (_previousValues.Count >= 5)
            _previousValues.RemoveAt(0);
        _previousValues.Add(_value);
        _value = newValue;
    }
}