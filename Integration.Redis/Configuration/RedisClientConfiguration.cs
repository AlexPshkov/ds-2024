using Integration.Redis.RegionSharding;

namespace Integration.Redis.Configuration;

public class RedisClientConfiguration
{
    public static string SectionName = nameof( RedisClientConfiguration );

    public Dictionary<Region, RedisConnectionConfiguration> RegionConnections { get; set; }
}

public class RedisConnectionConfiguration
{
    public string Connection { get; set; }
    public int Database { get; set; }
}