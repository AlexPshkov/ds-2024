using EventsLogger.Consumers;
using Integration.Nats;
using Integration.Nats.Configuration;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Nats.Messages.Implementation.Similarity;
using Integration.Redis;
using Integration.Redis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventsLogger;

public static class Program
{
    public static void Main( string[] args )
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder( args );

        ConfigurationManager configuration = builder.Configuration;

        NatsConfiguration? natsConfiguration = configuration.GetSection( NatsConfiguration.SectionName ).Get<NatsConfiguration>();

        builder.Services
            .AddNatsClient( natsConfiguration )
            .AddNatsConsumer<SimilarityCalculated, SimilarityCalculatedConsumer>()
            .AddNatsConsumer<RankCalculated, RankCalculatedConsumer>();

        BuildAndRun( builder );
    }

    private static void BuildAndRun( HostApplicationBuilder builder )
    {
        using IHost host = builder.Build();

        host.Run();
    }
}