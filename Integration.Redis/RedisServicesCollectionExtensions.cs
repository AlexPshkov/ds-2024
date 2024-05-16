using Integration.Redis.ClientFactory;
using Integration.Redis.Configuration;
using Integration.Redis.RegionSharding;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Integration.Redis;

public static class RedisServicesCollectionExtensions
{
    private const Region DefaultRegion = Region.RUS;

    public static void AddRedisClient( this IServiceCollection services, RedisClientConfiguration? configuration )
    {
        if ( configuration == null )
        {
            throw new ArgumentNullException( nameof( configuration ), "Redis configuration required" );
        }

        LoadConfiguration( configuration );

        services.AddSingleton( configuration );

        services.AddTransient<IRedisShardingClientFactory, RedisShardingClientFactory>();
    }

    /// <summary>
    /// Используется для хранения AntiForgeryToken
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddRedisDataProtection( this IServiceCollection services, RedisClientConfiguration? configuration )
    {
        if ( configuration == null )
        {
            throw new ArgumentNullException( nameof( configuration ), "Redis configuration required" );
        }

        if ( !configuration.RegionConnections.TryGetValue( DefaultRegion, out RedisConnectionConfiguration? defaultConnectionConfiguration ) )
        {
            throw new ArgumentNullException( nameof( configuration ), $"Can't find default region connection. Default region {DefaultRegion}" );
        }

        ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect( defaultConnectionConfiguration.Connection );
        services.AddDataProtection().PersistKeysToStackExchangeRedis( redisConnection, "DataProtection-Keys" );
    }

    /// <summary>
    /// Запихиваем в конфигурацию конекшены из переменных окружения 
    /// </summary>
    /// <param name="configuration">Конфигурация из файла</param>
    /// <exception cref="ArgumentException">Не найдена конфигурация (см. в сообщение ошибки)</exception>
    private static void LoadConfiguration( RedisClientConfiguration configuration )
    {
        foreach ( string regionName in Enum.GetNames<Region>() )
        {
            Region region = Enum.Parse<Region>( regionName );
            
            if (!configuration.RegionConnections.TryGetValue( region, out RedisConnectionConfiguration? connectionConfiguration ))
            {
                throw new ArgumentException( $"Can't find region configuration section for region {regionName}" );
            }

            string environmentVariable = $"DB_{regionName}";
            string? environmentDbConnection = Environment.GetEnvironmentVariable( environmentVariable );
            connectionConfiguration.Connection = String.Format( connectionConfiguration.Connection, environmentDbConnection );
            
            if ( String.IsNullOrEmpty( connectionConfiguration.Connection ) )
            {
                throw new ArgumentException( $"Can't find environment region connection for region {regionName}. Environment variable: {environmentVariable}" );
            }
        }
    }
}