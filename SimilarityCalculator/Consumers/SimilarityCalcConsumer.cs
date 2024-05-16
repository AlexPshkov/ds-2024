using System.Globalization;
using Infrastructure.Extensions;
using Infrastructure.RegionSharding;
using Integration.Nats.Client;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Similarity;
using Integration.Redis.Client;
using Integration.Redis.ClientFactory;
using Integration.Redis.RegionSharding;
using Microsoft.Extensions.Logging;

namespace SimilarityCalculator.Consumers;

public class SimilarityCalcConsumer : BasicConsumer<SimilarityCalcMessageRequest>
{
    private readonly IRedisShardingClientFactory _redisShardingClientFactory;
    private readonly INatsClient _natsClient;
    private readonly ILogger<SimilarityCalcConsumer> _logger;

    public SimilarityCalcConsumer( 
        IRedisShardingClientFactory redisShardingClientFactory, 
        INatsClient natsClient, 
        ILogger<SimilarityCalcConsumer> logger ) : base( natsClient )
    {
        _redisShardingClientFactory = redisShardingClientFactory;
        _natsClient = natsClient;
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( SimilarityCalcMessageRequest similarityCalcMessageRequest )
    {
        string stringId = similarityCalcMessageRequest.TextKey;
        Country country = similarityCalcMessageRequest.Country;
        
        _logger.LogInformation( $"LOOKUP: {stringId}, {country.GetRegion()}" );
        
        IRedisClient redisClient = _redisShardingClientFactory.GetClient( country );
        int similarity = CalculateSimilarity( redisClient, stringId.AsTextKey() );
        
        redisClient.Save( stringId.AsSimilarityKey(), similarity.ToString( CultureInfo.CurrentCulture ) );

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
    
    private int CalculateSimilarity( IRedisClient redisClient, string textKey )
    {
        string text = redisClient.Get( textKey );
        
        bool isSuch = redisClient.GetAllKeys()
            .Any( x => x.IsTextKey() && x != textKey && redisClient.Get( x ).Equals( text, StringComparison.CurrentCultureIgnoreCase ) );

        return isSuch ? 1 : 0;
    }
}