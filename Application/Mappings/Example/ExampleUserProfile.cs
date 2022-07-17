using Domain.Entities.Example;
using Shared.Responses.Example;

namespace Application.Mappings.Example;

public class ExampleUserProfile : BaseMapProfile
{
    public ExampleUserProfile()
    {
        CreateMap<ExampleUser, ExampleUserResponse>().ReverseMap();
    }
}