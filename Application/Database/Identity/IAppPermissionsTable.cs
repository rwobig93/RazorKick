namespace Application.Database.Identity;

public interface IAppPermissionsTable
{
    // Purpose of this interface is to be inherited when creating a new SQL provider, inherit to populate members then write implementation
    // TODO: Convert all table interfaces to static abstract members and update the inheriting classes
    static abstract SqlTable Table { get; }
    static abstract SqlStoredProcedure Delete { get; }
    static abstract SqlStoredProcedure DeleteForUser { get; }
    static abstract SqlStoredProcedure DeleteForRole { get; }
    static abstract SqlStoredProcedure GetAll { get; }
    static abstract SqlStoredProcedure GetAllPaginated { get; }
    static abstract SqlStoredProcedure GetById { get; }
    static abstract SqlStoredProcedure GetByName { get; }
    static abstract SqlStoredProcedure GetByGroup { get; }
    static abstract SqlStoredProcedure GetByAccess { get; }
    static abstract SqlStoredProcedure GetByClaimValue { get; }
    static abstract SqlStoredProcedure GetByRoleId { get; }
    static abstract SqlStoredProcedure GetByRoleIdAndValue { get; }
    static abstract SqlStoredProcedure GetByUserId { get; }
    static abstract SqlStoredProcedure GetByUserIdAndValue { get; }
    static abstract SqlStoredProcedure Insert { get; }
    static abstract SqlStoredProcedure Search { get; }
    static abstract SqlStoredProcedure SearchPaginated { get; }
    static abstract SqlStoredProcedure Update { get; }
}