using Integration.Nats.Client;
using Integration.Nats.Configuration;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Newtonsoft.Json;

namespace Integration.Nats;

public static class NatsServicesCollectionExtensions
{
    public static IServiceCollection AddNatsClient( this IServiceCollection services, NatsConfiguration? configuration )
    {
        if ( configuration == null )
        {
            throw new ArgumentNullException( nameof( configuration ), "MessageBus configuration required" );
        }

        configuration.NatsUrl = String.Format( configuration.NatsUrl, Environment.GetEnvironmentVariable( "NATS_URL" ) );

        services.AddSingleton<NatsConfiguration>();

        Options? defaultOptions = ConnectionFactory.GetDefaultOptions();
        defaultOptions.Url = configuration.NatsUrl;
        
        Console.WriteLine( "Using Nats url: " + defaultOptions.Url );
        
        services.AddSingleton( defaultOptions );
        services.AddSingleton<ConnectionFactory>();

        services.AddSingleton<IConnection>( sp =>
        {
            Options options = sp.GetRequiredService<Options>();
            ConnectionFactory connectionFactory = sp.GetRequiredService<ConnectionFactory>();
            
            return connectionFactory.CreateConnection( options );;
        } );

        services.AddScoped<INatsClient, NatsClient>();

        return services;
    }

    public static IServiceCollection AddNatsConsumer<T1, T2>( this IServiceCollection services ) 
        where T1 : IEventMessage 
        where T2 : BasicConsumer<T1>
    {
        services.AddHostedService<T2>();

        return services;
    }
}