namespace Domain.Models.Database;

public class DatabaseActionResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = "";
}

public class DatabaseActionResult<T> : DatabaseActionResult
{
    public T? Result { get; set; }
}