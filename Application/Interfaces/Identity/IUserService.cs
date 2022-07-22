using Application.Wrappers;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;
using IResult = Application.Wrappers.IResult;

namespace Application.Interfaces.Identity;

public interface IUserService
{
    Task<IEnumerable<AppUser>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<AppUser?> GetByIdAsync(GetUserByIdRequest userRequest);
    Task<AppUser?> GetByUsernameAsync(GetUserByUsernameRequest userRequest);
    Task UpdateAsync(UpdateUserRequest userRequest);
    Task DeleteAsync(DeleteUserRequest userRequest);
    // TODO: AddToRoleAsync, GetRolesAsync, ConfirmEmailAsync, ForgotPasswordAsync, ResetPasswordAsync, ExportToExcelAsync

    Task<IResult<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<IResult<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshRequest);

    Task<IResult<IdentityResult>> CreateAsync(UserCreateRequest createRequest, string password);

    Task<IResult<List<IdentityResult>>> AddToRolesAsync(UserRoleRequest roleRequest);

    Task<IResult<List<IdentityResult>>> RemoveFromRolesAsync(UserRoleRequest roleRequest);

    Task<IResult<IdentityResult>> EnforceRolesAsync(UserRoleRequest roleRequest);

    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);

    Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest);

    Task<IResult<List<RoleResponse>>> GetRolesAsync(Guid userId);

    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);

    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request);

    Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);
}