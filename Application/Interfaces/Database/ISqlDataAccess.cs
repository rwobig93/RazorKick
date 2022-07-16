namespace Application.Interfaces.Database;

public interface ISqlDataAccess
{
    public void EnsureDatabaseStructure(string connectionId = "DefaultConnection");
    
    public Task<IEnumerable<TDataClass>> LoadData<TDataClass, TParameters>(
        string storedProcedure,
        TParameters parameters,
        string connectionId = "DefaultConnection");

    public Task SaveData<TParameters>(
        string storedProcedure,
        TParameters parameters,
        string connectionId = "DefaultConnection");
}