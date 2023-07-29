namespace Application.Database.Tables.Shared;

public interface IGeneralTable : ISqlEnforcedEntity
{
    public static readonly SqlStoredProcedure GetRowCount = null!;
    public static readonly SqlStoredProcedure VerifyConnectivity = null!;
}