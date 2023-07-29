namespace Application.Database.Tables.Identity;

public interface IAppUserExtendedAttributesTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure Delete = null!;
    public static readonly SqlStoredProcedure DeleteAllForOwner = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByOwnerId = null!;
    public static readonly SqlStoredProcedure GetByName = null!;
    public static readonly SqlStoredProcedure GetByTypeAndValue = null!;
    public static readonly SqlStoredProcedure GetByTypeAndValueForOwner = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetAllOfType = null!;
    public static readonly SqlStoredProcedure GetAllOfTypeForOwner = null!;
    public static readonly SqlStoredProcedure GetAllOfNameForOwner = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Update = null!;
}