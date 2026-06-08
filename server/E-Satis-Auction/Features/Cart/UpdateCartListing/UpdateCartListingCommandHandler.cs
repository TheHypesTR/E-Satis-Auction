using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Cart.UpdateCartListing;

public sealed class UpdateCartListingCommandHandler : ICommandHandler<UpdateCartListingCommand, CartDto>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCartListingCommandHandler(
        IShoppingCartRepository cartRepository,
        ICommerceWorkflowService commerceWorkflowService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _commerceWorkflowService = commerceWorkflowService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartDto> Handle(UpdateCartListingCommand command, CancellationToken cancellationToken)
    {
        string userId = _currentUserService.UserId;
        ForbiddenAccessException.ThrowIfTrue(string.IsNullOrWhiteSpace(userId), ErrorMessages.Auth.UnauthorizedAccess, ErrorMessages.Exception.UnauthorizedAccess);

        Models.Commerce.ShoppingCart? cart = await _cartRepository.GetActiveByUserIdAsync(userId, enableTracking: true, cancellationToken);
        if (cart is null)
        {
            cart = Models.Commerce.ShoppingCart.Create(userId, command.Payload.ProductListingId, command.Payload.Quantity);
            await _cartRepository.AddAsync(cart, cancellationToken);
        }
        else
        {
            cart.ReplaceListing(command.Payload.ProductListingId, command.Payload.Quantity);
            _cartRepository.Update(cart);
        }

        CartPricePreviewDto preview = await _commerceWorkflowService.PreviewCartAsync(cart, cancellationToken);
        cart.UpdatePreview(preview.SubtotalAmount, preview.DiscountAmount, preview.ShippingAmount, preview.TotalAmount, preview.Currency);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToCartDto(cart);
    }
}
