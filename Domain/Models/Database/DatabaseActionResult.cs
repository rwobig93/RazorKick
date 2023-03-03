
namespace Domain.Models.Database;

public class DatabaseActionResult
{
    public bool Success { get; private set; }
    public string ErrorMessage { get; private set; } = "";

    public void Succeed()
    {
        Success = true;
    }

    public void Fail(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public void FailLog(ILogger logger, string pathOrAggregateId, string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
        logger.Error("DB Action Fail: [{SqlPathOrAggId}]: {ErrorMessage}", pathOrAggregateId, errorMessage);
    }
}

public class DatabaseActionResult<T> : DatabaseActionResult
{
    public T? Result { get; set; }

    public void Succeed(T result)
    {
        Succeed();
        Result = result;
    }
}