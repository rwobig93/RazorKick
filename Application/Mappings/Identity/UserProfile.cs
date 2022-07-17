using Domain.Entities.Identity;
using Shared.Requests.Identity;

namespace Application.Mappings.Identity;

public class UserProfile : BaseMapProfile
{
    public UserProfile()
    {
        CreateMap<User, UserRegisterRequest>().ReverseMap();
    }
}