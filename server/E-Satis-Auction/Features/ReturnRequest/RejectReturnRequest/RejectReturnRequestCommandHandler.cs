using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ReturnRequest.RejectReturnRequest;

using ReturnRequestEntity = Models.Commerce.ReturnRequest;

public sealed class RejectReturnRequestCommandHandler : ICommandHandler<RejectReturnRequestCommand>
{
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectReturnRequestCommandHandler(IReturnRequestRepository returnRequestRepository, IUnitOfWork unitOfWork)
    {
        _returnRequestRepository = returnRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RejectReturnRequestCommand command, CancellationToken cancellationToken)
    {
        ReturnRequestEntity? returnRequest = await _returnRequestRepository.GetByIdWithLinesAsync(command.ReturnRequestId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(returnRequest, ErrorMessages.ReturnRequest.EntityName, command.ReturnRequestId);

        returnRequest!.Reject(command.Payload.Reason);
        _returnRequestRepository.Update(returnRequest);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
