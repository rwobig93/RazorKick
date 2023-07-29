namespace Application.Database.Tables.Identity;

public interface IAppUserPreferencesTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure Delete = null!;
    public static readonly SqlStoredProcedure DeleteForUser = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByOwnerId = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Update = null!;
}