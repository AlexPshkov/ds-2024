using Infrastructure.Extensions;
using Infrastructure.RegionSharding;
using Integration.Redis.Client;
using Integration.Redis.Configuration;
using Integration.Redis.RegionSharding;
using StackExchange.Redis;

namespace Integration.Redis.ClientFactory;

public class RedisShardingClientFactory : IRedisShardingClientFactory
{
    private readonly RedisClientConfiguration _redisClientConfiguration;

    public RedisShardingClientFactory( RedisClientConfiguration redisClientConfiguration )
    {
        _redisClientConfiguration = redisClientConfiguration;
    }
    
    public IRedisClient GetClient( Country country )
    {
        Region region = country.GetRegion();
        
        if ( !_redisClientConfiguration.RegionConnections.TryGetValue( region, out RedisConnectionConfiguration? redisConnectionConfiguration ) )
        {
            throw new ArgumentException( $"Can't find region connection for region {region.ToString()}" );
        }
        
        IDatabase database = ConnectionMultiplexer
            .Connect( redisConnectionConfiguration.Connection )
            .GetDatabase( redisConnectionConfiguration.Database );

         IServer server = ConnectionMultiplexer
            .Connect( redisConnectionConfiguration.Connection )
            .GetServer( redisConnectionConfiguration.Connection );

         return new RedisClient( database, server );
    }
}