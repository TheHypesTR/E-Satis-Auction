using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Cart.ApplyCartCoupon;

public sealed class ApplyCartCouponCommandHandler : ICommandHandler<ApplyCartCouponCommand, CartDto>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ApplyCartCouponCommandHandler(
        IShoppingCartRepository cartRepository,
        ICampaignRepository campaignRepository,
        ICommerceWorkflowService commerceWorkflowService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _campaignRepository = campaignRepository;
        _commerceWorkflowService = commerceWorkflowService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<CartDto> Handle(ApplyCartCouponCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.ShoppingCart? cart = await _cartRepository.GetActiveByUserIdAsync(_currentUserService.UserId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(cart, ErrorMessages.Cart.EntityName, _currentUserService.UserId);

        Models.Commerce.Campaign? coupon = await _campaignRepository.GetActiveCouponByCodeAsync(command.Payload.CouponCode, DateTimeOffset.UtcNow, cancellationToken);
        NotFoundException.ThrowIfNull(coupon, ErrorMessages.Campaign.EntityName, command.Payload.CouponCode);

        cart!.ApplyCoupon(coupon!.Id);
        CartPricePreviewDto preview = await _commerceWorkflowService.PreviewCartAsync(cart, cancellationToken);
        cart.UpdatePreview(preview.SubtotalAmount, preview.DiscountAmount, preview.ShippingAmount, preview.TotalAmount, preview.Currency);
        _cartRepository.Update(cart);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToCartDto(cart);
    }
}
