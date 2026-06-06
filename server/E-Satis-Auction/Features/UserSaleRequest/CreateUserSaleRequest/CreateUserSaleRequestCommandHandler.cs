using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.UserSaleRequest.CreateUserSaleRequest;

public sealed class CreateUserSaleRequestCommandHandler : ICommandHandler<CreateUserSaleRequestCommand, UserSaleRequestDto>
{
    private readonly IUserSaleRequestRepository _userSaleRequestRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserSaleRequestCommandHandler(IUserSaleRequestRepository userSaleRequestRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _userSaleRequestRepository = userSaleRequestRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<UserSaleRequestDto> Handle(CreateUserSaleRequestCommand command, CancellationToken cancellationToken)
    {
        ForbiddenAccessException.ThrowIfTrue(string.IsNullOrWhiteSpace(_currentUserService.UserId), ErrorMessages.Auth.UnauthorizedAccess, ErrorMessages.Exception.UnauthorizedAccess);
        Models.Categories.Category? category = await _categoryRepository.GetByIdAsync(command.Payload.CategoryId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.Payload.CategoryId);

        Models.Commerce.UserSaleRequest request = Models.Commerce.UserSaleRequest.Create(
            _currentUserService.UserId,
            command.Payload.Title,
            command.Payload.Description,
            command.Payload.CategoryId,
            command.Payload.UserEstimatedValue);

        await _userSaleRequestRepository.AddAsync(request, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToUserSaleRequestDto(request);
    }
}
