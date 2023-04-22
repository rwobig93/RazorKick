using Application.Models.Lifecycle;

namespace Application.Helpers.Lifecycle;

public static class AuditHelpers
{
    public static AuditDiff GetAuditDiff<T>(T beforeObj, T afterObj)
    {
        var auditDiff = new AuditDiff();
        
        var beforeDict = beforeObj?.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(beforeObj)?.ToString());

        var afterDict = afterObj!.GetType().GetProperties()
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(afterObj)?.ToString());

        var beforeChangedDict = new Dictionary<string, string>();

        if (beforeDict is null)
        {
            auditDiff.Before = beforeChangedDict;
            auditDiff.After = afterDict!;
            return auditDiff;
        }
        
        var changedProps = beforeDict.Keys.Intersect(afterDict.Keys)
            .Where(key => 
                afterDict[key] is not null &&
                beforeDict[key] != afterDict[key]
                ).ToList();

        if (changedProps.Count < 1)
        {
            auditDiff.Before = beforeChangedDict;
            auditDiff.After = afterDict!;
            return auditDiff;
        }

        auditDiff.Before = changedProps.ToDictionary(prop => prop, prop => beforeDict[prop])!;
        auditDiff.After = changedProps.ToDictionary(prop => prop, prop => afterDict[prop])!;

        return auditDiff;
    }
}