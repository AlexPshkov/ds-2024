using System.Globalization;
using Infrastructure.Extensions;
using Integration.Nats.Client;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Similarity;
using Integration.Redis.Client;
using Microsoft.Extensions.Logging;

namespace SimilarityCalculator.Consumers;

public class SimilarityCalcConsumer : BasicConsumer<SimilarityCalcMessageRequest>
{
    private readonly IRedisClient _redisClient;
    private readonly INatsClient _natsClient;
    private readonly ILogger<SimilarityCalcConsumer> _logger;

    public SimilarityCalcConsumer( 
        IRedisClient redisClient, 
        INatsClient natsClient, 
        ILogger<SimilarityCalcConsumer> logger ) : base( natsClient )
    {
        _redisClient = redisClient;
        _natsClient = natsClient;
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( SimilarityCalcMessageRequest similarityCalcMessageRequest )
    {
        string stringId = similarityCalcMessageRequest.TextKey;
        
        int similarity = CalculateSimilarity( stringId.AsTextKey() );
        
        _redisClient.Save( stringId.AsSimilarityKey(), similarity.ToString( CultureInfo.CurrentCulture ) );

        _natsClient.Publish( new SimilarityCalculated
        {
            TextKey = similarityCalcMessageRequest.TextKey,
            Similarity = similarity
        } );
        
        _logger.LogInformation( $"Successfully handled {similarityCalcMessageRequest.TextKey} by machine {@Environment.MachineName}" );
        
        return new CalcMessageResponse
        {
            TextKey = stringId,
            IsSuccess = true
        };
    }
    
    private int CalculateSimilarity( string textKey )
    {
        string text = _redisClient.Get( textKey );
        
        bool isSuch = _redisClient.GetAllKeys()
            .Any( x => x.IsTextKey() && x != textKey && _redisClient.Get( x ).Equals( text, StringComparison.CurrentCultureIgnoreCase ) );

        return isSuch ? 1 : 0;
    }
}