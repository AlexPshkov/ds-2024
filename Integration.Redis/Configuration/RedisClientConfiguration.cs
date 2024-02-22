namespace Integration.Redis.Configuration;

public class RedisClientConfiguration
{
    public static string SectionName = nameof( RedisClientConfiguration );
    
    public string Connection { get; set; }
    public int Database { get; set; }
}