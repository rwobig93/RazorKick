using Application.Mappers.Lifecycle;
using Application.Models.Lifecycle;
using Application.Models.Web;
using Application.Repositories.Lifecycle;
using Application.Services.Lifecycle;
using Application.Services.System;
using Domain.DatabaseEntities.Lifecycle;
using Domain.Enums.Lifecycle;

namespace Infrastructure.Services.Lifecycle;

public class AuditTrailService : IAuditTrailService
{
    private readonly IAuditTrailsRepository _auditRepository;
    private readonly ISerializerService _serializer;
    private readonly ILogger _logger;

    public AuditTrailService(IAuditTrailsRepository auditRepository, ISerializerService serializer, ILogger logger)
    {
        _auditRepository = auditRepository;
        _serializer = serializer;
        _logger = logger;
    }

    private AuditTrailSlim ConvertToSlim(AuditTrailWithUserDb auditTrailDb)
    {
        var convertedTrail = auditTrailDb.ToSlim();

        if (string.IsNullOrWhiteSpace(auditTrailDb.After))
            return convertedTrail;
        
        try
        {
            convertedTrail.Before = string.IsNullOrWhiteSpace(auditTrailDb.Before)
                ? new Dictionary<string, string>()
                : _serializer
                    .Deserialize<Dictionary<string, string>>(auditTrailDb.Before);
            convertedTrail.After = _serializer.Deserialize<Dictionary<string, string>>(auditTrailDb.After);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to deserialize audit trail diff: {AuditTrailBefore} > {AuditTrailAfter}",
                auditTrailDb.Before, auditTrailDb.After);
            convertedTrail.After = new Dictionary<string, string>();
        }

        return convertedTrail;
    }

    public async Task<IResult<IEnumerable<AuditTrailSlim>>> GetAllAsync()
    {
        try
        {
            var auditTrails = await _auditRepository.GetAllWithUsersAsync();
            if (!auditTrails.Success)
                return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(auditTrails.ErrorMessage);

            var convertedAuditTrails = auditTrails.Result!.Select(ConvertToSlim).ToList();

            return await Result<IEnumerable<AuditTrailSlim>>.SuccessAsync(convertedAuditTrails);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AuditTrailSlim>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var auditTrails = await _auditRepository.GetAllPaginatedWithUsersAsync(pageNumber, pageSize);
            if (!auditTrails.Success)
                return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(auditTrails.ErrorMessage);

            var convertedAuditTrails = auditTrails.Result!.Select(ConvertToSlim).ToList();

            return await Result<IEnumerable<AuditTrailSlim>>.SuccessAsync(convertedAuditTrails);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<int>> GetCountAsync()
    {
        try
        {
            var auditTrailCount = await _auditRepository.GetCountAsync();
            if (!auditTrailCount.Success)
                return await Result<int>.FailAsync(auditTrailCount.ErrorMessage);

            return await Result<int>.SuccessAsync(auditTrailCount.Result);
        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<AuditTrailSlim?>> GetByIdAsync(Guid id)
    {
        try
        {
            var auditTrail = await _auditRepository.GetByIdWithUserAsync(id);
            if (!auditTrail.Success)
                return await Result<AuditTrailSlim?>.FailAsync(auditTrail.ErrorMessage);

            var convertedAuditTrail = ConvertToSlim(auditTrail.Result!);

            return await Result<AuditTrailSlim?>.SuccessAsync(convertedAuditTrail);
        }
        catch (Exception ex)
        {
            return await Result<AuditTrailSlim?>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AuditTrailSlim>>> GetByChangedByIdAsync(Guid id)
    {
        try
        {
            var auditTrails = await _auditRepository.GetByChangedByIdAsync(id);
            if (!auditTrails.Success)
                return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(auditTrails.ErrorMessage);

            var convertedAuditTrail = auditTrails.Result!.Select(ConvertToSlim).ToList();

            return await Result<IEnumerable<AuditTrailSlim>>.SuccessAsync(convertedAuditTrail);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AuditTrailSlim>>> GetByRecordIdAsync(Guid id)
    {
        try
        {
            var auditTrails = await _auditRepository.GetByRecordIdAsync(id);
            if (!auditTrails.Success)
                return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(auditTrails.ErrorMessage);

            var convertedAuditTrail = auditTrails.Result!.Select(ConvertToSlim).ToList();

            return await Result<IEnumerable<AuditTrailSlim>>.SuccessAsync(convertedAuditTrail);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<Guid>> CreateAsync(AuditTrailCreate createObject, bool systemUpdate = false)
    {
        try
        {
            var createTrail = await _auditRepository.CreateAsync(createObject, systemUpdate);
            if (!createTrail.Success)
                return await Result<Guid>.FailAsync(createTrail.ErrorMessage);

            return await Result<Guid>.SuccessAsync(createTrail.Result);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AuditTrailSlim>>> SearchAsync(string searchText)
    {
        try
        {
            var auditTrails = await _auditRepository.SearchWithUserAsync(searchText);
            if (!auditTrails.Success)
                return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(auditTrails.ErrorMessage);

            var convertedAuditTrail = auditTrails.Result!.Select(ConvertToSlim).ToList();

            return await Result<IEnumerable<AuditTrailSlim>>.SuccessAsync(convertedAuditTrail);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<IEnumerable<AuditTrailSlim>>> SearchPaginatedAsync(string searchText, int pageNumber, int pageSize)
    {
        try
        {
            var auditTrails = await _auditRepository.SearchPaginatedWithUserAsync(searchText, pageNumber, pageSize);
            if (!auditTrails.Success)
                return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(auditTrails.ErrorMessage);

            var convertedAuditTrail = auditTrails.Result!.Select(ConvertToSlim).ToList();

            return await Result<IEnumerable<AuditTrailSlim>>.SuccessAsync(convertedAuditTrail);
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<AuditTrailSlim>>.FailAsync(ex.Message);
        }
    }

    public async Task<IResult<int>> DeleteOld(CleanupTimeframe olderThan)
    {
        try
        {
            var deleteTrails = await _auditRepository.DeleteOld(olderThan);
            if (!deleteTrails.Success)
                return await Result<int>.FailAsync(deleteTrails.ErrorMessage);

            return await Result<int>.SuccessAsync(deleteTrails.Result);
        }
        catch (Exception ex)
        {
            return await Result<int>.FailAsync(ex.Message);
        }
    }
}