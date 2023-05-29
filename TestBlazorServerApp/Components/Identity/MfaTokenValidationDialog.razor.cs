using Application.Constants.Communication;
using Application.Helpers.Identity;
using Application.Services.Identity;
using Application.Services.System;
using Microsoft.AspNetCore.Components;
using Shared.Requests.Identity.User;

namespace TestBlazorServerApp.Components.Identity;

public partial class MfaTokenValidationDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public string MfaKey { get; set; } = "";
    [Parameter] public string VerifyCodeMessage { get; set; } = "Please enter your MFA code";
    [Inject] private IMfaService MfaService { get; init; } = null!;

    private string _mfaCode = "";

    private void VerifyMfaCode()
    {
        var validToken = MfaService.IsPasscodeCorrect(_mfaCode, MfaKey, out var timeStampMatched);
        if (!validToken)
        {
            Snackbar.Add("MFA Code entered is invalid, please try again", Severity.Error);
            return;
        }
        
        // TODO: If code was matched previously we'll return a failure, this converts to a time that doesn't line up, need to troubleshoot
        var codeMatchedTime = DateTimeOffset.FromUnixTimeSeconds(timeStampMatched).DateTime;
        
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}