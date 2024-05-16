using Integration.Nats;
using Integration.Nats.Configuration;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Redis;
using Integration.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RankCalculator.Consumers;

namespace RankCalculator;

public static class Program
{
    public static void Main( string[] args )
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder( args );

        ConfigurationManager configuration = builder.Configuration;

        RedisClientConfiguration? redisClientConfiguration = configuration.GetSection( RedisClientConfiguration.SectionName ).Get<RedisClientConfiguration>();
        NatsConfiguration? natsConfiguration = configuration.GetSection( NatsConfiguration.SectionName ).Get<NatsConfiguration>();

        builder.Services.AddRedisClient( redisClientConfiguration );

        builder.Services
            .AddNatsClient( natsConfiguration )
            .AddNatsConsumer<RankCalcMessageRequest, RankCalcConsumer>();

        BuildAndRun( builder );
    }

    private static void BuildAndRun( HostApplicationBuilder builder )
    {
        using IHost host = builder.Build();

        host.Run();
    }
}