using Shared.Models.Base;

namespace Shared.Requests.Identity.User;

public class DeleteUserRequest : ApiObjectFromQuery<DeleteUserRequest>
{
    public Guid Id { get; set; }
}