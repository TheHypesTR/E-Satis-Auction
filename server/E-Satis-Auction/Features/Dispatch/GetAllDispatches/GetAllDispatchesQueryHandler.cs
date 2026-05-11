using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Dispatch;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Dispatch.GetAllDispatches;

using Models.Dispatches;

public sealed class GetAllDispatchesQueryHandler : IQueryHandler<GetAllDispatchesQuery, PaginatedList<DispatchSummaryDto>>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDispatchesQueryHandler(
        IDispatchRepository dispatchRepository,
        IFacilityRepository facilityRepository,
        ICurrentUserService currentUserService)
    {
        _dispatchRepository = dispatchRepository;
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<DispatchSummaryDto>> Handle(GetAllDispatchesQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Dispatch> dispatchQuery = _dispatchRepository.GetAllAsQueryable();
        dispatchQuery = await ApplyAuthorizationAsync(dispatchQuery, query, cancellationToken);
        dispatchQuery = ApplyFilters(dispatchQuery, query);

        PaginatedList<Dispatch> pagedDispatches = await dispatchQuery
            .OrderByDescending(d => d.UpdatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        if (pagedDispatches.Items.Count is 0)
        {
            return new PaginatedList<DispatchSummaryDto>([], pagedDispatches.TotalCount, query.PageNumber, query.PageSize);
        }

        IReadOnlyCollection<Dispatch> dispatches = pagedDispatches.Items;
        List<Guid> facilityIds = dispatches
            .SelectMany(d => new[] { d.SourceFacilityId, d.TargetFacilityId ?? Guid.Empty })
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        Dictionary<Guid, string> facilityNames = facilityIds.Count is 0
            ? new Dictionary<Guid, string>()
            : await _facilityRepository.GetFacilityNamesByIdsAsync(facilityIds, cancellationToken);

        List<DispatchSummaryDto> dtoList = dispatches
            .Select(d => MapToSummaryDto(d, facilityNames))
            .ToList();

        return new PaginatedList<DispatchSummaryDto>(dtoList, pagedDispatches.TotalCount, pagedDispatches.PageNumber, query.PageSize);
    }

    private async Task<IQueryable<Dispatch>> ApplyAuthorizationAsync(IQueryable<Dispatch> dispatchQuery, GetAllDispatchesQuery query, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsGeneralAdmin)
        {
            return dispatchQuery;
        }

        if (query.SourceFacilityId.HasValue)
        {
            bool hasSourceAccess = await _currentUserService.HasFacilityAccess(query.SourceFacilityId.Value, cancellationToken);
            ForbiddenAccessException.ThrowIfFalse(
                hasSourceAccess,
                ErrorMessages.Facility.UnauthorizedFacilityAccess,
                ErrorMessages.Exception.UnauthorizedAccess);
        }

        if (query.TargetFacilityId.HasValue)
        {
            bool hasTargetAccess = await _currentUserService.HasFacilityAccess(query.TargetFacilityId.Value, cancellationToken);
            ForbiddenAccessException.ThrowIfFalse(
                hasTargetAccess,
                ErrorMessages.Facility.UnauthorizedFacilityAccess,
                ErrorMessages.Exception.UnauthorizedAccess);
        }

        IReadOnlyCollection<Guid> facilityIds = await _currentUserService.GetAccessibleFacilityIdsAsync(cancellationToken);
        if (facilityIds.Count is 0)
        {
            return dispatchQuery.Where(_ => false);
        }

        return dispatchQuery.Where(d =>
            facilityIds.Contains(d.SourceFacilityId) ||
            (d.TargetFacilityId.HasValue && facilityIds.Contains(d.TargetFacilityId.Value)));
    }

    private static IQueryable<Dispatch> ApplyFilters(IQueryable<Dispatch> dispatchQuery, GetAllDispatchesQuery query)
    {
        if (query.SourceFacilityId.HasValue)
        {
            dispatchQuery = dispatchQuery.Where(d => d.SourceFacilityId == query.SourceFacilityId.Value);
        }

        if (query.TargetFacilityId.HasValue)
        {
            dispatchQuery = dispatchQuery.Where(d => d.TargetFacilityId == query.TargetFacilityId.Value);
        }

        if (query.Status.HasValue)
        {
            dispatchQuery = dispatchQuery.Where(d => d.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchTerm = query.SearchTerm.Trim().ToLower();
            dispatchQuery = dispatchQuery.Where(d =>
                d.TrackingNumber.ToLower().Contains(searchTerm) ||
                d.ReceiverName.ToLower().Contains(searchTerm));
        }

        return dispatchQuery;
    }

    private static DispatchSummaryDto MapToSummaryDto(Dispatch dispatch, Dictionary<Guid, string> facilityNames)
    {
        string sourceFacilityName = facilityNames.GetValueOrDefault(dispatch.SourceFacilityId, string.Empty);
        string? targetFacilityName = dispatch.TargetFacilityId.HasValue
            ? facilityNames.GetValueOrDefault(dispatch.TargetFacilityId.Value, string.Empty)
            : null;

        return new DispatchSummaryDto(
            dispatch.Id,
            dispatch.TrackingNumber,
            dispatch.Status,
            dispatch.SourceFacilityId,
            sourceFacilityName,
            dispatch.TargetFacilityId,
            targetFacilityName,
            dispatch.TargetAddressId,
            dispatch.ReceiverName,
            dispatch.ReceiverPhone,
            dispatch.DispatchDate,
            dispatch.CreatedAt,
            dispatch.UpdatedAt);
    }
}