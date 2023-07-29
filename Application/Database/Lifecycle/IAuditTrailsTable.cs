namespace Application.Database.Lifecycle;

public interface IAuditTrailsTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetAllWithUsers = null!;
    public static readonly SqlStoredProcedure GetAllPaginated = null!;
    public static readonly SqlStoredProcedure GetAllPaginatedWithUsers = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByIdWithUser = null!;
    public static readonly SqlStoredProcedure GetByChangedBy = null!;
    public static readonly SqlStoredProcedure GetByRecordId = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Search = null!;
    public static readonly SqlStoredProcedure SearchPaginated = null!;
    public static readonly SqlStoredProcedure SearchWithUser = null!;
    public static readonly SqlStoredProcedure SearchPaginatedWithUser = null!;
    public static readonly SqlStoredProcedure DeleteOlderThan = null!;
}