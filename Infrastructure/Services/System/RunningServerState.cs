using Application.Services.System;

namespace Infrastructure.Services.System;

public class RunningServerState : IRunningServerState
{
    public bool IsRunningInDebugMode { get; set; }
    public string ApplicationName { get; set; } = "";
    public Guid SystemUserId { get; set; } = Guid.Empty;
}