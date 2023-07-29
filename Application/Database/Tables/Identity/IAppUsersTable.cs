namespace Application.Database.Tables.Identity;

public interface IAppUsersTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure Delete = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetAllServiceAccountsForPermissions = null!;
    public static readonly SqlStoredProcedure GetAllPaginated = null!;
    public static readonly SqlStoredProcedure GetAllServiceAccountsPaginated = null!;
    public static readonly SqlStoredProcedure GetAllDisabledPaginated = null!;
    public static readonly SqlStoredProcedure GetAllLockedOutPaginated  = null!;
    public static readonly SqlStoredProcedure GetAllDeleted = null!;
    public static readonly SqlStoredProcedure GetAllLockedOut = null!;
    public static readonly SqlStoredProcedure GetByEmail = null!;
    public static readonly SqlStoredProcedure GetByEmailFull = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByIdSecurity = null!;
    public static readonly SqlStoredProcedure GetByIdFull = null!;
    public static readonly SqlStoredProcedure GetByUsername = null!;
    public static readonly SqlStoredProcedure GetByUsernameFull = null!;
    public static readonly SqlStoredProcedure GetByUsernameSecurity = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Search = null!;
    public static readonly SqlStoredProcedure SearchPaginated = null!;
    public static readonly SqlStoredProcedure Update = null!;
    public static readonly SqlStoredProcedure SetUserId = null!;
    public static readonly SqlStoredProcedure SetCreatedById = null!;
}