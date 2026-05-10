using System.Security.Cryptography;
using System.Web;
using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using Microsoft.AspNetCore.DataProtection;

namespace e_Sat_Auction.Extensions;

public static class DataProtectionExtension
{
    public static string GenerateUrlEncodedPayload(this IDataProtector protector, string userId, string token)
    {
        string rawPayload = $"{userId}|{token}";
        string encryptedPayload = protector.Protect(rawPayload);
        return HttpUtility.UrlEncode(encryptedPayload);
    }

    public static (string UserId, string Token) ExtractPayload(this IDataProtector protector, string encryptedPayload, string errorKey)
    {
        string decryptedPayload = protector.SafeUnprotect(encryptedPayload, errorKey);

        string[] payloadParts = decryptedPayload.Split('|');
        if (payloadParts.Length != 2)
        {
            throw new BusinessException(errorKey, ErrorMessages.Exception.PayloadTitle);
        }

        return (payloadParts[0], payloadParts[1]);
    }

    private static string SafeUnprotect(
        this IDataProtector protector,
        string encryptedPayload,
        string errorKey = ErrorMessages.Validation.InvalidResetLink)
    {
        try
        {
            return protector.Unprotect(encryptedPayload);
        }
        catch (CryptographicException)
        {
            throw new BusinessException(errorKey, ErrorMessages.Exception.PayloadTitle);
        }
    }
}