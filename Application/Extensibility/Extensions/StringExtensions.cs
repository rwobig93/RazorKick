namespace Application.Extensibility.Extensions;

public static class StringExtensions
{
    public static string DboFullName(this string objectName) => $"dbo.{objectName.Replace(".sql", "")}";
    public static string DbScriptPath(this string objectName, string path) => $"{path}{objectName}";
}