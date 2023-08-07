namespace Application.Database.Lifecycle;

public interface IServerStateRecordsTable
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetAllBeforeDate = null!;
    public static readonly SqlStoredProcedure GetAllAfterDate = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByVersion = null!;
    public static readonly SqlStoredProcedure Insert = null!;
}