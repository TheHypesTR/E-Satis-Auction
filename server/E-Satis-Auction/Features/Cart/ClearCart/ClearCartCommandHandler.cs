using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Cart.ClearCart;

public sealed class ClearCartCommandHandler : ICommandHandler<ClearCartCommand>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ClearCartCommandHandler(IShoppingCartRepository cartRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ClearCartCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.ShoppingCart? cart = await _cartRepository.GetActiveByUserIdAsync(_currentUserService.UserId, enableTracking: true, cancellationToken);
        if (cart is null)
        {
            return;
        }

        cart.Clear();
        _cartRepository.Update(cart);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
