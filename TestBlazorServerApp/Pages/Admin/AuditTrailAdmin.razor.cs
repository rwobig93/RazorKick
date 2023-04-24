using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Models.Lifecycle;
using Application.Services.Integrations;
using Application.Services.Lifecycle;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;

namespace TestBlazorServerApp.Pages.Admin;

public partial class AuditTrailAdmin
{
    [Inject] private IAuditTrailService AuditService { get; init; } = null!;
    [Inject] private IDateTimeService DateTimeService { get; init; } = null!;
    [Inject] private ISerializerService Serializer { get; init; } = null!;
    [Inject] private IExcelService ExcelService { get; init; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; init; } = null!;
    
    private MudTable<AuditTrailSlim> _table = new();
    private IEnumerable<AuditTrailSlim> _pagedData = new List<AuditTrailSlim>();
    private string _searchString = "";
    private int _totalTrails;
    // TODO: Gather local client timezone to inject
    private readonly TimeZoneInfo _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

    private bool _canSearchTrails;
    private bool _canExportTrails;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetPermissions();
            StateHasChanged();
        }
    }

    private async Task GetPermissions()
    {
        var currentUser = (await CurrentUserService.GetCurrentUserPrincipal())!;
        _canSearchTrails = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Audit.Search);
        _canExportTrails = await AuthorizationService.UserHasPermission(currentUser, PermissionConstants.Audit.Export);
    }
    
    private async Task<TableData<AuditTrailSlim>> ServerReload(TableState state)
    {
        var trailResult = await AuditService.SearchAsync(_searchString);
        if (!trailResult.Succeeded)
        {
            trailResult.Messages.ForEach(x => Snackbar.Add(x, Severity.Error));
            return new TableData<AuditTrailSlim>();
        }

        var data = trailResult.Data;

        data = data.ToArray();
        
        _totalTrails = data.Count();

        data = state.SortLabel switch
        {
            "Id" => data.OrderByDirection(state.SortDirection, o => o.Id),
            "Timestamp" => data.OrderByDirection(state.SortDirection, o => o.Timestamp),
            "RecordId" => data.OrderByDirection(state.SortDirection, o => o.RecordId),
            "Action" => data.OrderByDirection(state.SortDirection, o => o.Action),
            "TableName" => data.OrderByDirection(state.SortDirection, o => o.TableName),
            "ChangedByUsername" => data.OrderByDirection(state.SortDirection, o => o.ChangedByUsername),
            _ => data
        };

        _pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        
        return new TableData<AuditTrailSlim>() {TotalItems = _totalTrails, Items = _pagedData};
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private void ViewTrail(Guid trailId)
    {
        var viewUserUri = QueryHelpers.AddQueryString(AppRouteConstants.Admin.AuditTrailView, "trailId", trailId.ToString());
        NavManager.NavigateTo(viewUserUri);
    }

    private async Task ExportToExcel()
    {
        var convertedExcelWorkbook = await ExcelService.ExportBase64Async(_pagedData, dataMapping: new Dictionary<string, Func<AuditTrailSlim, object>>
        {
            { "Id", auditTrail => auditTrail.Id },
            { "Timestamp", auditTrail => auditTrail.Timestamp.ConvertToLocal(_localTimeZone).ToString(DataConstants.DateTime.DisplayFormat) },
            { "RecordId", auditTrail => auditTrail.RecordId.ToString() },
            { "Action", auditTrail => auditTrail.Action.ToString() },
            { "Type", auditTrail => auditTrail.TableName },
            { "ChangedById", auditTrail => auditTrail.ChangedBy.ToString() },
            { "ChangedByUsername", auditTrail => auditTrail.ChangedByUsername },
            { "Before", auditTrail => Serializer.Serialize(auditTrail.Before) },
            { "After", auditTrail => Serializer.Serialize(auditTrail.After) }
        }, sheetName: "AuditTrails");

        var fileName =
            $"AuditTrails_{DateTimeService.NowDatabaseTime.ConvertToLocal(_localTimeZone).ToString(DataConstants.DateTime.DisplayFormat)}.xlsx";
        
        await JsRuntime.InvokeVoidAsync("Download", new
        {
            ByteArray = convertedExcelWorkbook,
            FileName = fileName,
            MimeType = DataConstants.MimeTypes.OpenXml
        });

        Snackbar.Add("Successfully exported Audit Trails to Excel Workbook For Download", Severity.Success);
    }
}