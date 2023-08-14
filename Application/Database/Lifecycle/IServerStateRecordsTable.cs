namespace Application.Database.Lifecycle;

public interface IServerStateRecordsTable
{
    static abstract SqlTable Table { get; }
    static abstract SqlStoredProcedure GetAll { get; }
    static abstract SqlStoredProcedure GetAllBeforeDate { get; }
    static abstract SqlStoredProcedure GetAllAfterDate { get; }
    static abstract SqlStoredProcedure GetById { get; }
    static abstract SqlStoredProcedure GetByVersion { get; }
    static abstract SqlStoredProcedure Insert { get; }
}