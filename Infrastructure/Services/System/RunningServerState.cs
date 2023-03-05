using Application.Services.System;

namespace Infrastructure.Services.System;

public class RunningServerState : IRunningServerState
{
    public bool IsRunningInDebugMode { get; set; }
}