namespace Application.Database.Identity;

public interface IAppUserRoleJunctionsTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure Delete = null!;
    public static readonly SqlStoredProcedure DeleteForUser = null!;
    public static readonly SqlStoredProcedure DeleteForRole = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetByUserRoleId = null!;
    public static readonly SqlStoredProcedure GetRolesOfUser = null!;
    public static readonly SqlStoredProcedure GetUsersOfRole = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Search = null!;
}