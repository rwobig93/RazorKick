using Domain.Enums.Identity;

namespace Domain.DatabaseEntities.Identity;

public class Validator
{
    public Guid Id { get; set; }
    public string? Value { get; set; }
    public ValidatorType Type { get; set; }
}