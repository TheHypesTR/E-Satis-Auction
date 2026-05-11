using E_Satis_Auction.Common.Constants;

namespace E_Satis_Auction.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public string Title { get; }

    private ForbiddenAccessException(string message, string title = ErrorMessages.Exception.SecurityTitle) : base(message)
    {
        Title = title;
    }

    public static void ThrowIfTrue(bool condition, string message, string title = ErrorMessages.Exception.SecurityTitle)
    {
        if (condition)
        {
            throw new ForbiddenAccessException(message, title);
        }
    }
    
    public static void ThrowIfFalse(bool condition, string message, string title = ErrorMessages.Exception.SecurityTitle)
    {
        if (!condition)
        {
            throw new ForbiddenAccessException(message, title);
        }
    }
}