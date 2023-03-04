using Application.Services.Lifecycle;

namespace Infrastructure.Services.Lifecycle;

public class RunningServerState : IRunningServerState
{
    public bool IsRunningInDebugMode { get; set; }
}