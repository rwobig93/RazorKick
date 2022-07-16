using Shared.Models.Base;

namespace Shared.ApiRequests.Example;

public class DeleteUserRequest : ApiObjectFromQuery<DeleteUserRequest>
{
    public int Id { get; set; }
}