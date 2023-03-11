using Application.Constants.Communication;
using Application.Models.Email;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Application.Helpers.Communication;

public static class EmailHelpers
{
    public static async Task<SendResponse> SendRegistrationEmail(this IFluentEmail mailService, string emailAddress, string username, string 
    confirmationUrl)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), EmailConstants.TemplatesPath,
            EmailConstants.PathRegistrationConfirmation);
        
        return await mailService.Subject("Registration Confirmation").To(emailAddress)
            .UsingTemplateFromFile(templatePath, new EmailAction() 
                {ActionUrl = confirmationUrl, Username = username})
            .SendAsync();
    }
    
    public static async Task<SendResponse> SendPasswordResetEmail(this IFluentEmail mailService, string emailAddress, string username, string 
        confirmationUrl)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), EmailConstants.TemplatesPath,
            EmailConstants.PathPasswordReset);
        
        return await mailService.Subject("Password Reset Confirmation").To(emailAddress)
            .UsingTemplateFromFile(templatePath, new EmailAction() 
                {ActionUrl = confirmationUrl, Username = username})
            .SendAsync();
    }
}