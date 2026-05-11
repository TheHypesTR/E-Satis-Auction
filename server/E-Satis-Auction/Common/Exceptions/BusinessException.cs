using E_Satis_Auction.Common.Constants;

namespace E_Satis_Auction.Common.Exceptions;

public class BusinessException : Exception
{
    public string Title { get; }

    public BusinessException(string message, string title = ErrorMessages.Exception.BusinessErrorTitle) : base(message)
    {
        Title = title;
    }

    public static void ThrowIfNull(object? obj, string message, string title = ErrorMessages.Exception.BusinessErrorTitle)
    {
        if (obj is null)
        {
            throw new BusinessException(message, title);
        }
    }

    public static void ThrowIfNotNull(object? obj, string message, string title = ErrorMessages.Exception.BusinessErrorTitle)
    {
        if (obj is not null)
        {
            throw new BusinessException(message, title);
        }
    }

    public static void ThrowIfTrue(bool condition, string message, string title = ErrorMessages.Exception.BusinessErrorTitle)
    {
        if (condition)
        {
            throw new BusinessException(message, title);
        }
    }

    public static void ThrowIfFalse(bool condition, string message, string title = ErrorMessages.Exception.BusinessErrorTitle)
    {
        if (!condition)
        {
            throw new BusinessException(message, title);
        }
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string message, string title = ErrorMessages.Exception.BusinessErrorTitle)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessException(message, title);
        }
    }
}