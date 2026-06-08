using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Features.Cart.GetCart;

public sealed class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartDto?>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ICommerceWorkflowService _commerceWorkflowService;
    private readonly ICurrentUserService _currentUserService;

    public GetCartQueryHandler(
        IShoppingCartRepository cartRepository,
        ICommerceWorkflowService commerceWorkflowService,
        ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _commerceWorkflowService = commerceWorkflowService;
        _currentUserService = currentUserService;
    }

    public async Task<CartDto?> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.ShoppingCart? cart = await _cartRepository.GetActiveByUserIdAsync(_currentUserService.UserId, cancellationToken: cancellationToken);
        if (cart is null)
        {
            return null;
        }

        CartPricePreviewDto preview = await _commerceWorkflowService.PreviewCartAsync(cart, cancellationToken);
        return CommerceDtoMapper.ToCartDto(cart, preview);
    }
}
