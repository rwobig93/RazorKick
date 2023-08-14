namespace Application.Database.Identity;

public interface IAppUserSecurityAttributesTable
{
    static abstract SqlTable Table { get; }
    static abstract SqlStoredProcedure Delete { get; }
    static abstract SqlStoredProcedure DeleteAllForOwner { get; }
    static abstract SqlStoredProcedure GetById { get; }
    static abstract SqlStoredProcedure GetByOwnerId { get; }
    static abstract SqlStoredProcedure GetAll { get; }
    static abstract SqlStoredProcedure Insert { get; }
    static abstract SqlStoredProcedure Update { get; }
    static abstract SqlStoredProcedure SetOwnerId { get; }
}