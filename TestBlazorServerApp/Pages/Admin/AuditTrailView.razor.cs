using System.Security.Claims;
using Application.Constants.Identity;
using Application.Constants.Web;
using Application.Helpers.Runtime;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Lifecycle;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Application.Services.System;
using FluentEmail.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using TestBlazorServerApp.Components.Identity;

namespace TestBlazorServerApp.Pages.Admin;

public partial class AuditTrailView
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IAuditTrailService AuditService { get; init; } = null!;
    [Inject] private ISerializerService Serializer { get; init; } = null!;
    
    [Parameter] public Guid TrailId { get; set; }

    private AuditTrailSlim _viewingTrail = new();
    // TODO: Gather local client timezone to inject
    private readonly TimeZoneInfo _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

    private bool _invalidDataProvided;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                ParseParametersFromUri();
                await GetViewingAuditTrail();
                StateHasChanged();
            }
        }
        catch
        {
            _invalidDataProvided = true;
            StateHasChanged();
        }
    }

    private void ParseParametersFromUri()
    {
        var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
        var queryParameters = QueryHelpers.ParseQuery(uri.Query);

        if (!queryParameters.TryGetValue("trailId", out var queryTrailId)) return;
        
        var providedIdIsValid = Guid.TryParse(queryTrailId, out var parsedTrailId);
        if (!providedIdIsValid)
            throw new InvalidDataException("Invalid TrailId provided for audit trail view");
            
        TrailId = parsedTrailId;
    }

    private async Task GetViewingAuditTrail()
    {
        _viewingTrail = (await AuditService.GetByIdAsync(TrailId)).Data!;
    }

    private void GoBack()
    {
        NavManager.NavigateTo(AppRouteConstants.Admin.AuditTrails);
    }
}