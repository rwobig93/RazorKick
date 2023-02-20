using Application.Models.Web;
using Domain.DatabaseEntities.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Application.Repositories.Identity;

public interface IAppRoleRepository
{
    Task<IResult<List<RoleResponse>>> GetAllAsync();
    
    Task<IResult<int>> GetCountAsync();
    
    Task<AppRoleDb?> GetByIdAsync(string id);
    
    Task<AppRoleDb?> GetByNameAsync(string roleName);
    
    Task<IResult<string>> CreateAsync(CreateRoleRequest request);
    
    Task<IResult<string>> UpdateAsync(UpdateRoleRequest request);
    
    Task<IResult<string>> DeleteAsync(string id);
    
    Task<IResult<PermissionsResponse>> GetAllPermissionsAsync(string roleId);
    
    Task<IResult<string>> AddPermissionsAsync(PermissionsRequest request);
    
    Task<IResult<string>> RemovePermissionsAsync(PermissionsRequest request);
    
    Task<IResult<string>> EnforcePermissionsAsync(PermissionsRequest request);
}
