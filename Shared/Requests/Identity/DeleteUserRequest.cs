using Shared.Models.Base;

namespace Shared.Requests.Identity;

public class DeleteUserRequest : ApiObjectFromQuery<DeleteExampleUserRequest>
{
    public Guid Id { get; set; }
}