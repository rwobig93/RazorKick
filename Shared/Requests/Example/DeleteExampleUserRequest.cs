using Shared.Models.Base;

namespace Shared.Requests.Example;

public class DeleteExampleUserRequest : ApiObjectFromQuery<DeleteExampleUserRequest>
{
    public int Id { get; set; }
}