using Microsoft.AspNetCore.Authorization;

namespace Application.Settings.Identity;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    public PermissionAuthorizationHandler()
    { }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // TODO: Removed conditional for null user as user *shouldn't* be null, need to verify all edge cases
        if (context.User is null)
        {
            context.Fail();
            await Task.CompletedTask;
            return;
        }
        
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
