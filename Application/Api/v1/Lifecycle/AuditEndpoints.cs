using Application.Constants.Communication;
using Application.Helpers.Web;
using Application.Models.Identity;
using Application.Models.Lifecycle;
using Application.Models.Web;
using Application.Services.Identity;
using Application.Services.Lifecycle;
using Domain.Enums.Lifecycle;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Identity.User;
using Shared.Responses.Identity;
using Shared.Responses.Lifecycle;
using Shared.Routes;

namespace Application.Api.v1.Lifecycle;

public static class AuditEndpoints
{
    public static void MapEndpointsAudit(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Lifecycle.Audit.GetAll, GetAllTrails).ApiVersionOne();
        app.MapGet(ApiRoutes.Lifecycle.Audit.GetById, GetAuditTrailById).ApiVersionOne();
        app.MapGet(ApiRoutes.Lifecycle.Audit.GetByChangedBy, GetAuditTrailsByChangedBy).ApiVersionOne();
        app.MapGet(ApiRoutes.Lifecycle.Audit.GetByRecordId, GetAuditTrailsByRecordId).ApiVersionOne();
        app.MapDelete(ApiRoutes.Lifecycle.Audit.Delete, Delete).ApiVersionOne();
        
        // TODO: Add swagger endpoint viewer enrichment
    }

    private static async Task<IResult<List<AuditTrailResponse>>> GetAllTrails(IAuditTrailService auditService)
    {
        try
        {
            var allAuditTrails = await auditService.GetAllAsync();
            if (!allAuditTrails.Succeeded)
                return await Result<List<AuditTrailResponse>>.FailAsync(allAuditTrails.Messages);

            return await Result<List<AuditTrailResponse>>.SuccessAsync(allAuditTrails.Data.ToResponses().ToList());
        }
        catch (Exception ex)
        {
            return await Result<List<AuditTrailResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<AuditTrailResponse>> GetAuditTrailById([FromQuery]Guid id, IAuditTrailService auditService)
    {
        try
        {
            var foundAuditTrail = await auditService.GetByIdAsync(id);
            if (!foundAuditTrail.Succeeded)
                return await Result<AuditTrailResponse>.FailAsync(foundAuditTrail.Messages);

            if (foundAuditTrail.Data is null)
                return await Result<AuditTrailResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<AuditTrailResponse>.SuccessAsync(foundAuditTrail.Data.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<AuditTrailResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<AuditTrailResponse>>> GetAuditTrailsByChangedBy([FromQuery]Guid id, IAuditTrailService auditService)
    {
        try
        {
            var auditTrails = await auditService.GetByChangedByIdAsync(id);
            if (!auditTrails.Succeeded)
                return await Result<List<AuditTrailResponse>>.FailAsync(auditTrails.Messages);

            return await Result<List<AuditTrailResponse>>.SuccessAsync(auditTrails.Data.ToResponses().ToList());
        }
        catch (Exception ex)
        {
            return await Result<List<AuditTrailResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<AuditTrailResponse>>> GetAuditTrailsByRecordId([FromQuery]Guid id, IAuditTrailService auditService)
    {
        try
        {
            var auditTrails = await auditService.GetByRecordIdAsync(id);
            if (!auditTrails.Succeeded)
                return await Result<List<AuditTrailResponse>>.FailAsync(auditTrails.Messages);

            return await Result<List<AuditTrailResponse>>.SuccessAsync(auditTrails.Data.ToResponses().ToList());
        }
        catch (Exception ex)
        {
            return await Result<List<AuditTrailResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> Delete(CleanupTimeframe olderThan, IAuditTrailService auditService)
    {
        try
        {
            var deleteRequest = await auditService.DeleteOld(olderThan);
            if (!deleteRequest.Succeeded) return deleteRequest;
            return await Result.SuccessAsync("Successfully cleaned up old records!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}