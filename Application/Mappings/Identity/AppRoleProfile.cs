using Domain.Entities.Identity;
using Shared.Responses.Identity;

namespace Application.Mappings.Identity;

public class AppRoleProfile : BaseMapProfile
{
    public AppRoleProfile()
    {
        CreateMap<AppRole, RoleResponse>().ReverseMap();
    }
}