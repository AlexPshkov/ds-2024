using Infrastructure.RegionSharding;
using Integration.Redis.Client;

namespace Integration.Redis.ClientFactory;

public interface IRedisShardingClientFactory
{
    IRedisClient GetClient( Country country );
}