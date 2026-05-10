using e_Sat_Auction.Common.Constants;

namespace e_Sat_Auction.Common.Exceptions;

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