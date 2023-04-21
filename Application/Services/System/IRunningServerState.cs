namespace Application.Services.System;

public interface IRunningServerState
{
    public bool IsRunningInDebugMode { get; set; }
    public string ApplicationName { get; set; }
    public Guid SystemUserId { get; set; }
    public bool AuditLoginLogout { get; set; }
}