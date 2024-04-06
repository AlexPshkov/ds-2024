using Integration.Nats.Client;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Similarity;
using Microsoft.Extensions.Logging;

namespace EventsLogger.Consumers;

public class SimilarityCalculatedConsumer : BasicConsumer<SimilarityCalculated>
{
    private readonly ILogger<SimilarityCalculatedConsumer> _logger;

    public SimilarityCalculatedConsumer( 
        INatsClient natsClient, 
        ILogger<SimilarityCalculatedConsumer> logger ) : base( natsClient )
    {
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( SimilarityCalculated similarityCalcMessageRequest )
    {
        _logger.LogInformation( "Получено новое событие" +
                                $"\n1. Название события: {nameof(SimilarityCalculated)}" +
                                $"\n2. Идентификатор контекста: {similarityCalcMessageRequest.TextKey}" +
                                $"\n3. Значение similarity для события: {similarityCalcMessageRequest.Similarity}" );

        return new CommonResponse { IsSuccess = true };
    }
}