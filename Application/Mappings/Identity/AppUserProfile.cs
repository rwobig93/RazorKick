using Domain.Entities.Identity;
using Shared.Requests.Identity;

namespace Application.Mappings.Identity;

public class AppUserProfile : BaseMapProfile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, UserRegisterRequest>().ReverseMap();
    }
}