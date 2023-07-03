using Application.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Application.Services.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    public PermissionAuthorizationHandler()
    { }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // TODO: Look at pulling full permissions list or how to inject permissions separately to get real-time permission validation
        var permissions = context.User!.Claims.Where(x => x.Type == ApplicationClaimTypes.Permission &&
                                                          x.Value == requirement.Permission &&
                                                          x.Issuer == "LOCAL AUTHORITY");
        if (permissions.Any())
        {
            context.Succeed(requirement);
            await Task.CompletedTask;
        }
    }
}
