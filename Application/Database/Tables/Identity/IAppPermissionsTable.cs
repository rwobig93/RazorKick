namespace Application.Database.Tables.Identity;

public interface IAppPermissionsTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure Delete = null!;
    public static readonly SqlStoredProcedure DeleteForUser = null!;
    public static readonly SqlStoredProcedure DeleteForRole = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetAllPaginated = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByName = null!;
    public static readonly SqlStoredProcedure GetByGroup = null!;
    public static readonly SqlStoredProcedure GetByAccess = null!;
    public static readonly SqlStoredProcedure GetByRoleId = null!;
    public static readonly SqlStoredProcedure GetByRoleIdAndValue = null!;
    public static readonly SqlStoredProcedure GetByUserId = null!;
    public static readonly SqlStoredProcedure GetByUserIdAndValue = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Search = null!;
    public static readonly SqlStoredProcedure SearchPaginated = null!;
    public static readonly SqlStoredProcedure Update = null!;
}