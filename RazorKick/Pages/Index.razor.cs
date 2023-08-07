using Application.Constants.Identity;
using Application.Helpers.Runtime;
using Application.Models.Identity.User;
using Application.Services.Lifecycle;

namespace RazorKick.Pages;

public partial class Index
{
    // MainLayout has a CascadingParameter of itself, this allows the refresh button on the AppBar to refresh all page state data
    //  If this parameter isn't cascaded to a page then the refresh button won't affect that pages' state data
    [CascadingParameter] public MainLayout ParentLayout { get; set; } = null!;
    
    [Inject] private IAppAccountService AccountService { get; init; } = null!;
    [Inject] private IRunningServerState ServerState { get; init; } = null!;

    private AppUserFull _loggedInUser = new();
    
    private bool _canViewApi;
    private bool _canViewJobs;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await UpdateLoggedInUser();
                await GetPermissions();
                StateHasChanged();
            }
        }
        catch
        {
            // User has old saved token so we'll force a local storage clear and de-authenticate then redirect due to being unauthorized
            await AccountService.LogoutGuiAsync(Guid.Empty);
            StateHasChanged();
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = await CurrentUserService.GetCurrentUserPrincipal();
        _canViewApi = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Api.View);
        _canViewJobs = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Jobs.View);
    }

    private async Task UpdateLoggedInUser()
    {
        var user = await CurrentUserService.GetCurrentUserFull();
        if (user is null)
            return;

        _loggedInUser = user;
    }
    
    // TODO: Look into deserialization failure for audit trail viewing
    // 08/07/2023 17:12:45 -05:00 [Warning] Failed to deserialize audit trail diff: "{}" > "{\n  \"id\": \"4e3b2cae-06c3-437d-9086-b1c24b10406a\",\n  \"username\": \"Anonymous\",\n  \"emailAddress\": \"Who@am.i\",\n  \"firstName\": \"Anonymous\",\n  \"lastName\": \"User\",\n  \"createdBy\": \"48903d50-cc28-4d49-aa00-f068eaa52409\",\n  \"profilePictureDataUrl\": null,\n  \"createdOn\": \"2023-08-07T22:11:05.9066667\",\n  \"lastModifiedBy\": null,\n  \"lastModifiedOn\": null,\n  \"isDeleted\": false,\n  \"deletedOn\": null,\n  \"accountType\": 0,\n  \"notes\": null,\n  \"authState\": 1\n}"
    // 08/07/2023 17:12:45 -05:00 [Warning] Failed to deserialize audit trail diff: "{}" > "{\n  \"id\": \"caee08af-bbf0-42b9-bda7-078090a619db\",\n  \"username\": \"Superperson\",\n  \"emailAddress\": \"Superperson@localhost.local\",\n  \"firstName\": \"Admini\",\n  \"lastName\": \"Strator\",\n  \"createdBy\": \"48903d50-cc28-4d49-aa00-f068eaa52409\",\n  \"profilePictureDataUrl\": null,\n  \"createdOn\": \"2023-08-07T22:11:05.3033333\",\n  \"lastModifiedBy\": null,\n  \"lastModifiedOn\": null,\n  \"isDeleted\": false,\n  \"deletedOn\": null,\n  \"accountType\": 0,\n  \"notes\": null,\n  \"authState\": 1\n}"
    // 08/07/2023 17:12:56 -05:00 [Warning] Failed to deserialize audit trail diff: "{}" > "{\n  \"id\": \"4e3b2cae-06c3-437d-9086-b1c24b10406a\",\n  \"username\": \"Anonymous\",\n  \"emailAddress\": \"Who@am.i\",\n  \"firstName\": \"Anonymous\",\n  \"lastName\": \"User\",\n  \"createdBy\": \"48903d50-cc28-4d49-aa00-f068eaa52409\",\n  \"profilePictureDataUrl\": null,\n  \"createdOn\": \"2023-08-07T22:11:05.9066667\",\n  \"lastModifiedBy\": null,\n  \"lastModifiedOn\": null,\n  \"isDeleted\": false,\n  \"deletedOn\": null,\n  \"accountType\": 0,\n  \"notes\": null,\n  \"authState\": 1\n}"
    // 08/07/2023 17:12:56 -05:00 [Warning] Failed to deserialize audit trail diff: "{}" > "{\n  \"id\": \"caee08af-bbf0-42b9-bda7-078090a619db\",\n  \"username\": \"Superperson\",\n  \"emailAddress\": \"Superperson@localhost.local\",\n  \"firstName\": \"Admini\",\n  \"lastName\": \"Strator\",\n  \"createdBy\": \"48903d50-cc28-4d49-aa00-f068eaa52409\",\n  \"profilePictureDataUrl\": null,\n  \"createdOn\": \"2023-08-07T22:11:05.3033333\",\n  \"lastModifiedBy\": null,\n  \"lastModifiedOn\": null,\n  \"isDeleted\": false,\n  \"deletedOn\": null,\n  \"accountType\": 0,\n  \"notes\": null,\n  \"authState\": 1\n}"
    // 08/07/2023 17:13:03 -05:00 [Warning] Failed to deserialize audit trail diff: "{}" > "{\n  \"id\": \"4e3b2cae-06c3-437d-9086-b1c24b10406a\",\n  \"username\": \"Anonymous\",\n  \"emailAddress\": \"Who@am.i\",\n  \"firstName\": \"Anonymous\",\n  \"lastName\": \"User\",\n  \"createdBy\": \"48903d50-cc28-4d49-aa00-f068eaa52409\",\n  \"profilePictureDataUrl\": null,\n  \"createdOn\": \"2023-08-07T22:11:05.9066667\",\n  \"lastModifiedBy\": null,\n  \"lastModifiedOn\": null,\n  \"isDeleted\": false,\n  \"deletedOn\": null,\n  \"accountType\": 0,\n  \"notes\": null,\n  \"authState\": 1\n}"
    // 08/07/2023 17:13:06 -05:00 [Warning] Failed to deserialize audit trail diff: "{}" > "{\n  \"id\": \"4e3b2cae-06c3-437d-9086-b1c24b10406a\",\n  \"username\": \"Anonymous\",\n  \"emailAddress\": \"Who@am.i\",\n  \"firstName\": \"Anonymous\",\n  \"lastName\": \"User\",\n  \"createdBy\": \"48903d50-cc28-4d49-aa00-f068eaa52409\",\n  \"profilePictureDataUrl\": null,\n  \"createdOn\": \"2023-08-07T22:11:05.9066667\",\n  \"lastModifiedBy\": null,\n  \"lastModifiedOn\": null,\n  \"isDeleted\": false,\n  \"deletedOn\": null,\n  \"accountType\": 0,\n  \"notes\": null,\n  \"authState\": 1\n}"
}