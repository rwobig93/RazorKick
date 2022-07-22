using Application.Wrappers;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Application.Interfaces.Identity;

public interface IClaimService
{
    Task<IResult<List<ClaimResponse>>> GetAllAsync();

    Task<IResult<int>> GetCountAsync();

    Task<IResult<ClaimResponse>> GetByIdAsync(int id);

    Task<IResult<List<ClaimResponse>>> GetAllByRoleIdAsync(string roleId);

    Task<IResult<string>> SaveAsync(ClaimRequest request);

    Task<IResult<string>> DeleteAsync(int id);
}
