using E_Satis_Auction.Common.Constants;

namespace E_Satis_Auction.Common.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object Key { get; }

    private NotFoundException(string entityName, object key) : base(ErrorMessages.Exception.NotFoundMessage)
    {
        EntityName = entityName;
        Key = key;
    }

    public static void ThrowIfNull(object? obj, string entityName, object key)
    {
        if (obj is null)
        {
            throw new NotFoundException(entityName, key);
        }
    }
}