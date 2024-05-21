using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Nats.Messages.Implementation.Similarity;
using NATS.Client;

namespace Integration.Nats.Client;

public interface INatsClient
{
    Task<CalcMessageResponse?>MakeCalcRankRequest( RankCalcMessageRequest rankCalcMessageRequest );
    void Publish<T>( T eventMessage ) where T : IEventMessage;
    Task<T2?> RequestAsync<T1, T2>( T1 requestMessage ) where T1 : IEventMessage where T2 : IEventMessageResult;

    IAsyncSubscription SubscribeAsync( string subject, string queueName, EventHandler<MsgHandlerEventArgs> handler );
}