using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.UserSaleRequest.ApproveUserSaleRequest;

public sealed class ApproveUserSaleRequestCommandHandler : ICommandHandler<ApproveUserSaleRequestCommand, UserSaleRequestDto>
{
    private readonly IUserSaleRequestRepository _userSaleRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveUserSaleRequestCommandHandler(IUserSaleRequestRepository userSaleRequestRepository, IUnitOfWork unitOfWork)
    {
        _userSaleRequestRepository = userSaleRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserSaleRequestDto> Handle(ApproveUserSaleRequestCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.UserSaleRequest? request = await _userSaleRequestRepository.GetByIdAsync(command.RequestId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(request, ErrorMessages.UserSaleRequest.EntityName, command.RequestId);
        request!.Approve(command.Payload.AcquisitionPrice, command.Payload.TargetResalePrice, command.Payload.AdminNote);
        _userSaleRequestRepository.Update(request);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToUserSaleRequestDto(request);
    }
}
