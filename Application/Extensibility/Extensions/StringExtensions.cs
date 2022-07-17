using Application.Constants.Database;

namespace Application.Extensibility.Extensions;

public static class StringExtensions
{
    public static string ToDboName(this string objectName) => 
        $"dbo.{objectName.Replace(".sql", "")}";

    public static string ToDbScriptPathTable(this string dbScript) =>
        $"{MsSqlConstants.PathTables}{dbScript.Split("_")[0].Replace("sp", "")}.{dbScript}";

    public static string ToDbScriptPathTable(this object dbScript) =>
        $"{MsSqlConstants.PathTables}{((string) dbScript).Split("_")[0].Replace("sp", "")}.{dbScript}";
}