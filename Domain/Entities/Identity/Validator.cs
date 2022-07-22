using Domain.Enums.Identity;

namespace Domain.Entities.Identity;

public class Validator
{
    public Guid Id { get; set; }
    public string? Value { get; set; }
    public ValidatorType Type { get; set; }
}