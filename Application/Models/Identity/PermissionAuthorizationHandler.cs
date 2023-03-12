using Microsoft.AspNetCore.Authorization;

namespace Application.Models.Identity;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    public PermissionAuthorizationHandler()
    { }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
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
