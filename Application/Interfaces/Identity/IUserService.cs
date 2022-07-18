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

    Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest loginRequest);
    Task<Result<UserLoginResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshRequest);

    Task<IResult<IdentityResult>> CreateAsync(UserRegisterRequest registerRequest, string password);

    Task<IResult<IdentityResult>> AddToRolesAsync(UserRoleRequest roleRequest);

    Task<IResult<IdentityResult>> RemoveFromRolesAsync(UserRoleRequest roleRequest);

    Task<IResult> EnforceRolesAsync(UserRoleRequest roleRequest);

    Task<IResult> RegisterAsync(UserRegisterRequest registerRequest);

    Task<IResult> ToggleUserStatusAsync(ChangeUserActiveStateRequest activeRequest);

    Task<IResult<List<RoleResponse>>> GetRolesAsync(Guid userId);

    Task<IResult<string>> ConfirmEmailAsync(Guid userId, string confirmationCode);

    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request);

    Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);
}