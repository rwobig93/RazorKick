using Shared.Models.Base;

namespace Shared.Requests.Identity;

public class DeleteUserRequest : ApiObjectFromQuery<DeleteUserRequest>
{
    public Guid Id { get; set; }
}