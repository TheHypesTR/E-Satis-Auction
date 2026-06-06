using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Cart.RemoveCartCoupon;

public sealed class RemoveCartCouponCommandHandler : ICommandHandler<RemoveCartCouponCommand, CartDto>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemoveCartCouponCommandHandler(IShoppingCartRepository cartRepository, ICommerceWorkflowService commerceWorkflowService, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _commerceWorkflowService = commerceWorkflowService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartDto> Handle(RemoveCartCouponCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.ShoppingCart? cart = await _cartRepository.GetActiveByUserIdAsync(_currentUserService.UserId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(cart, ErrorMessages.Cart.EntityName, _currentUserService.UserId);

        cart!.RemoveCoupon();
        CartPricePreviewDto preview = await _commerceWorkflowService.PreviewCartAsync(cart, cancellationToken);
        cart.UpdatePreview(preview.SubtotalAmount, preview.DiscountAmount, preview.ShippingAmount, preview.TotalAmount, preview.Currency);
        _cartRepository.Update(cart);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToCartDto(cart);
    }
}
