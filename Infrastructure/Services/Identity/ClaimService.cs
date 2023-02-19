using Application.Models.Web;
using Application.Services.Identity;
using Shared.Requests.Identity;
using Shared.Responses.Identity;

namespace Infrastructure.Services.Identity;

public class ClaimService : IClaimService
{
    public async Task<IResult<List<ClaimResponse>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<int>> GetCountAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<ClaimResponse>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<List<ClaimResponse>>> GetAllByRoleIdAsync(string roleId)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> SaveAsync(ClaimRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<string>> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}