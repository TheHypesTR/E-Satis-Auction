using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Cart.GetCartPricePreview;

public sealed class GetCartPricePreviewQueryHandler : IQueryHandler<GetCartPricePreviewQuery, CartPricePreviewDto>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public GetCartPricePreviewQueryHandler(IShoppingCartRepository cartRepository, ICommerceWorkflowService commerceWorkflowService, ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _commerceWorkflowService = commerceWorkflowService;
        _currentUserService = currentUserService;
    }

    public async Task<CartPricePreviewDto> Handle(GetCartPricePreviewQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.ShoppingCart? cart = await _cartRepository.GetActiveByUserIdAsync(_currentUserService.UserId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(cart, ErrorMessages.Cart.EntityName, _currentUserService.UserId);

        return await _commerceWorkflowService.PreviewCartAsync(cart!, cancellationToken);
    }
}
