namespace E_Satis_Auction.Dtos.Auth;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime Expiration);