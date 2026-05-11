namespace E_Satis_Auction.Common.Options;

public class RedisOptions
{
    public const string SectionName = "RedisSettings";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
}