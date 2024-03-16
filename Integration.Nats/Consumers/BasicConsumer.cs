using System.Text;
using Integration.Nats.Messages;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Newtonsoft.Json;

namespace Integration.Nats.Consumers;

public abstract class BasicConsumer<T> : IHostedService where T : IEventMessage
{
    private readonly IConnection _connection;

    private IAsyncSubscription? _subscription;

    public BasicConsumer( IConnection connection )
    {
        _connection = connection;
    }
    
    public abstract IEventMessageResult Handle( T calcMessageRequest );

    public Task StartAsync( CancellationToken cancellationToken )
    {
        _subscription = _connection.SubscribeAsync( typeof(T).FullName, typeof(T).Name, ( sender, args ) =>
        {
            T? eventMessage = JsonConvert.DeserializeObject<T>( Encoding.UTF8.GetString( args.Message.Data ) );

            if ( eventMessage == null )
            {
                return;
            }
                
            IEventMessageResult eventMessageResult = Handle( eventMessage );
            
            args.Message.Respond( Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( eventMessageResult ) ) );
        } );
        
        _subscription.Start();
        
        return Task.CompletedTask;
    }

    public Task StopAsync( CancellationToken cancellationToken )
    {
        _subscription?.Dispose();
        
        return Task.CompletedTask;
    }
}