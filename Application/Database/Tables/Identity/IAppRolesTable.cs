namespace Application.Database.Tables.Identity;

public interface IAppRolesTable : ISqlEnforcedEntity
{
    public static readonly SqlTable Table = null!;
    public static readonly SqlStoredProcedure Delete = null!;
    public static readonly SqlStoredProcedure GetAll = null!;
    public static readonly SqlStoredProcedure GetAllPaginated = null!;
    public static readonly SqlStoredProcedure GetById = null!;
    public static readonly SqlStoredProcedure GetByName = null!;
    public static readonly SqlStoredProcedure Insert = null!;
    public static readonly SqlStoredProcedure Search = null!;
    public static readonly SqlStoredProcedure SearchPaginated = null!;
    public static readonly SqlStoredProcedure Update = null!;
    public static readonly SqlStoredProcedure SetCreatedById = null!;
}