using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;

namespace Integration.Nats.Client;

public interface INatsClient
{
    Task<CalcMessageResponse?>MakeCalcRequest( CalcMessageRequest calcMessageRequest );
    
    void Publish<T>( T eventMessage ) where T : IEventMessage;
    Task<T2?> RequestAsync<T1, T2>( T1 requestMessage ) where T1 : IEventMessage where T2 : IEventMessageResult;
}