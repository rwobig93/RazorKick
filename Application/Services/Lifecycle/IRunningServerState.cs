namespace Application.Services.Lifecycle;

public interface IRunningServerState
{
    public bool IsRunningInDebugMode { get; set; }
}