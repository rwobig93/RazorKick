using Shared.Models.Base;
using Shared.Requests.Example;

namespace Shared.Requests.Identity;

public class DeleteUserRequest : ApiObjectFromQuery<DeleteExampleUserRequest>
{
    public Guid Id { get; set; }
}