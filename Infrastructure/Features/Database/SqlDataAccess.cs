using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Application.Constants.Database;
using Application.Extensibility.Extensions;
using Application.Interfaces.Database;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Features.Database;

public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public SqlDataAccess(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void EnsureDatabaseStructure(string connectionId)
    {
        EnsureTablesExistence(connectionId);
        EnsureStoredProceduresExistence(connectionId);
    }

    public async Task<IEnumerable<TDataClass>> LoadData<TDataClass, TParameters>(
        string storedProcedure,
        TParameters parameters,
        string connectionId)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

        return await connection.QueryAsync<TDataClass>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveData<TParameters>(
        string storedProcedure,
        TParameters parameters,
        string connectionId)
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));

        await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    private void EnsureTablesExistence(string connectionId)
    {
        var databaseTables = GetAllTablesFullPaths();
        foreach (var tablePath in databaseTables)
        {
            ExecuteEmbeddedSqlScript(connectionId, tablePath);
        }
    }

    private void EnsureStoredProceduresExistence(string connectionId)
    {
        var storedProcedures = GetAllStoredProceduresFullPaths();
        foreach (var storedProcedurePath in storedProcedures)
        {
            ExecuteEmbeddedSqlScript(connectionId, storedProcedurePath);
        }
    }

    private void ExecuteEmbeddedSqlScript(string connectionId, string scriptPath)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(scriptPath);
            using var reader = new StreamReader(stream!);
            var procedureContents = reader.ReadToEnd();
            using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionId));
            connection.Execute(procedureContents);
            _logger.Debug("Successfully ran embedded SQL script: {scriptPath}", scriptPath);
        }
        catch (Exception ex)
        {
            _logger.Error("Failure occurred attempting to run embedded SQL script: {errorMessage}", ex.Message);
        }
    }

    private static List<string> GetAllTablesFullPaths()
    {
        var fields = typeof(MsSqlConstants.Tables)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return (from fi in fields select fi.GetValue(null)
            into propertyValue
            where propertyValue is not null select 
                $"{MsSqlConstants.PathTables}{propertyValue}").ToList()!;
    }

    private static List<string> GetAllStoredProceduresFullPaths()
    {
        var fields = typeof(MsSqlConstants.StoredProcedures)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return (from fi in fields select fi.GetValue(null)
            into propertyValue
            where propertyValue is not null select propertyValue.ToDbScriptPathTable()).ToList()!;
    }
}