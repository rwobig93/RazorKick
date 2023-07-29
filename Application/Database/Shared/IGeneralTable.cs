namespace Application.Database.Shared;

public interface IGeneralTable : ISqlEnforcedEntity
{
    public static readonly SqlStoredProcedure GetRowCount = null!;
    public static readonly SqlStoredProcedure VerifyConnectivity = null!;
}