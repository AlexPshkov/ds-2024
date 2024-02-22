using Integration.Redis.Client;
using Integration.Redis.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Integration.Redis;

public static class RedisServicesCollectionExtensions
{
    public static void AddRedisClient( this IServiceCollection services, RedisClientConfiguration? configuration )
    {
        if ( configuration == null )
        {
            throw new ArgumentNullException( nameof( configuration ), "Redis configuration required" );
        }

        configuration.Connection = string.Format( configuration.Connection,
            Environment.GetEnvironmentVariable( "REDIS_ADDRESS" ), 
            Environment.GetEnvironmentVariable( "REDIS_PORT" ) );

        services.AddSingleton<RedisClientConfiguration>();
        
        services.AddTransient<IDatabase>( x => ConnectionMultiplexer
            .Connect( configuration.Connection )
            .GetDatabase( configuration.Database ) );

        services.AddTransient<IServer>( x => ConnectionMultiplexer
            .Connect( configuration.Connection )
            .GetServer( configuration.Connection ) );

        services.AddTransient<IRedisClient, RedisClient>();
    }
}