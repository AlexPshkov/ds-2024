using Integration.Nats.Client;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Nats.Messages.Implementation.Similarity;
using Microsoft.Extensions.Logging;

namespace EventsLogger.Consumers;

public class RankCalculatedConsumer : BasicConsumer<RankCalculated>
{
    private readonly ILogger<RankCalculatedConsumer> _logger;

    public RankCalculatedConsumer( 
        INatsClient natsClient, 
        ILogger<RankCalculatedConsumer> logger ) : base( natsClient )
    {
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( RankCalculated similarityCalcMessageRequest )
    {
        _logger.LogInformation( "Получено новое событие" +
                                $"\n1. Название события: {nameof(RankCalculated)}" +
                                $"\n2. Идентификатор контекста: {similarityCalcMessageRequest.TextKey}" +
                                $"\n3. Значение rank для события: {similarityCalcMessageRequest.Rank}" );

        return new CommonResponse { IsSuccess = true };
    }
}