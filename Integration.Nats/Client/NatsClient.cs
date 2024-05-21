using System.Text;
using Integration.Nats.Configuration;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Nats.Messages.Implementation.Similarity;
using NATS.Client;
using Newtonsoft.Json;

namespace Integration.Nats.Client;

public class NatsClient : INatsClient
{
    private readonly IConnection _connection;
    private readonly NatsConfiguration _natsConfiguration;

    public NatsClient( IConnection connection, NatsConfiguration natsConfiguration )
    {
        _connection = connection;
        _natsConfiguration = natsConfiguration;
    }

    public async Task<CalcMessageResponse?>MakeCalcRankRequest( RankCalcMessageRequest rankCalcMessageRequest )
    {
        return await RequestAsync<RankCalcMessageRequest, CalcMessageResponse>( rankCalcMessageRequest );
    }
    
    public void Publish<T>( T eventMessage ) where T : IEventMessage
    {
        string serializeObject = JsonConvert.SerializeObject( eventMessage );
       _connection.Publish( typeof(T).FullName, Encoding.UTF8.GetBytes( serializeObject ) );
    }

    public async Task<T2?> RequestAsync<T1, T2>( T1 requestMessage ) where T1 : IEventMessage where T2 : IEventMessageResult
    {
        string serializeRequestObject = JsonConvert.SerializeObject( requestMessage );
        Msg? response = await _connection.RequestAsync( typeof(T1).FullName, Encoding.UTF8.GetBytes( serializeRequestObject ) );
        
        return response == null ? default : JsonConvert.DeserializeObject<T2>( Encoding.UTF8.GetString( response.Data ) );
    }

    public IAsyncSubscription SubscribeAsync( string subject, string queueName, EventHandler<MsgHandlerEventArgs> handler )
    {
        return _connection.SubscribeAsync( subject, queueName, handler );
    }
}