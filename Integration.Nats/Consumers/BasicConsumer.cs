using System.Text;
using Integration.Nats.Client;
using Integration.Nats.Messages;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Newtonsoft.Json;

namespace Integration.Nats.Consumers;

public abstract class BasicConsumer<T> : IHostedService where T : IEventMessage
{
    private readonly INatsClient _natsClient;

    private IAsyncSubscription? _subscription;

    public BasicConsumer( INatsClient natsClient )
    {
        _natsClient = natsClient;
    }
    
    public abstract IEventMessageResult Handle( T similarityCalcMessageRequest );

    public Task StartAsync( CancellationToken cancellationToken )
    {
        _subscription = _natsClient.SubscribeAsync( typeof(T).FullName!, typeof(T).Name, ( sender, args ) =>
        {
            T? eventMessage = JsonConvert.DeserializeObject<T>( Encoding.UTF8.GetString( args.Message.Data ) );

            if ( eventMessage == null )
            {
                return;
            }
                
            IEventMessageResult eventMessageResult = Handle( eventMessage );
            
            if ( !String.IsNullOrEmpty( args.Message.Reply ) )
            {
                args.Message.Respond( Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( eventMessageResult ) ) );
            }
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