using AutoMapper;
using Domain.Entities.Example;
using Shared.ApiResponses.Example;

namespace Application.Mappings.Example;

public class UserProfile : BaseMapProfile
{
    public UserProfile()
    {
        // CreateMap<User, UserResponse>().ReverseMap();
        CreateMap<User, UserResponse>();
        CreateMap<UserResponse, User>();
    }
}