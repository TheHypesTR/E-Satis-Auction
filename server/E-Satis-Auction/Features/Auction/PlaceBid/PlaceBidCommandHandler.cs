using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.Auction.PlaceBid;

using AuctionBidEntity = Models.Commerce.AuctionBid;
using AuctionEntity = Models.Commerce.Auction;

public sealed class PlaceBidCommandHandler : ICommandHandler<PlaceBidCommand, AuctionBidDto>
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IAuctionBidRepository _auctionBidRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuctionRealtimeNotifier _notifier;

    public PlaceBidCommandHandler(
        IAuctionRepository auctionRepository,
        IAuctionBidRepository auctionBidRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuctionRealtimeNotifier notifier)
    {
        _auctionRepository = auctionRepository;
        _auctionBidRepository = auctionBidRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _notifier = notifier;
    }

    public async Task<AuctionBidDto> Handle(PlaceBidCommand command, CancellationToken cancellationToken)
    {
        string userId = _currentUserService.UserId;
        ForbiddenAccessException.ThrowIfTrue(string.IsNullOrWhiteSpace(userId), ErrorMessages.Auth.UnauthorizedAccess, ErrorMessages.Exception.UnauthorizedAccess);

        AuctionBidEntity? existingBid = await _auctionBidRepository.GetByIdempotencyAsync(command.AuctionId, userId, command.Payload.IdempotencyKey, cancellationToken: cancellationToken);
        if (existingBid is not null)
        {
            return AuctionDtoMapper.ToBidDto(existingBid);
        }

        AuctionEntity? auction = await _auctionRepository.GetByIdWithDetailsAsync(command.AuctionId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(auction, ErrorMessages.Auction.EntityName, command.AuctionId);

        bool wasExtended;
        AuctionBidEntity bid = auction!.AcceptBid(
            userId,
            command.Payload.Amount,
            command.Payload.IdempotencyKey,
            DateTimeOffset.UtcNow,
            AuctionRules.AntiSnipeWindow,
            AuctionRules.AntiSnipeExtension,
            out wasExtended);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _auctionBidRepository.AddAsync(bid, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex) when (IsAuctionConcurrencyConflict(ex))
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new BusinessException(ErrorMessages.Auction.BidConcurrencyConflict, ErrorMessages.Exception.CommerceTitle);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        AuctionBidDto bidDto = AuctionDtoMapper.ToBidDto(bid);
        AuctionSnapshotDto snapshot = AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow);
        await _notifier.BroadcastBidAcceptedAsync(snapshot, bidDto, cancellationToken);
        if (wasExtended)
        {
            await _notifier.BroadcastAuctionExtendedAsync(snapshot, cancellationToken);
        }

        return bidDto;
    }

    private static bool IsAuctionConcurrencyConflict(DbUpdateConcurrencyException exception)
    {
        return exception.Entries.Count > 0 &&
               exception.Entries.All(entry => entry.Entity is AuctionEntity);
    }
}
