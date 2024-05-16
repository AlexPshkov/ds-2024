using System.Globalization;
using Infrastructure.Extensions;
using Infrastructure.RegionSharding;
using Integration.Nats.Client;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Redis.Client;
using Integration.Redis.ClientFactory;
using Microsoft.Extensions.Logging;

namespace RankCalculator.Consumers;

public class RankCalcConsumer : BasicConsumer<RankCalcMessageRequest>
{
    private readonly IRedisShardingClientFactory _redisShardingClientFactory;
    private readonly INatsClient _natsClient;
    private readonly ILogger<RankCalcConsumer> _logger;

    public RankCalcConsumer( 
        IRedisShardingClientFactory redisShardingClientFactory, 
        INatsClient natsClient, 
        ILogger<RankCalcConsumer> logger ) : base( natsClient )
    {
        _redisShardingClientFactory = redisShardingClientFactory;
        _natsClient = natsClient;
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( RankCalcMessageRequest similarityCalcMessageRequest )
    {
        string stringId = similarityCalcMessageRequest.TextKey;
        Country country = similarityCalcMessageRequest.Country;
        
        _logger.LogInformation( $"LOOKUP: {stringId}, {country.GetRegion()}" );
        
        IRedisClient redisClient = _redisShardingClientFactory.GetClient( country );
        string text = redisClient.Get( stringId.AsTextKey() );

        double rank = CalculateRank( text );
        redisClient.Save( stringId.AsRankKey(), rank.ToString( CultureInfo.CurrentCulture ) );

        _natsClient.Publish( new RankCalculated
        {
            TextKey = similarityCalcMessageRequest.TextKey,
            Rank = rank
        } );
        
        _logger.LogInformation( $"Successfully handled {similarityCalcMessageRequest.TextKey} by machine {@Environment.MachineName}" );
        
        return new CalcMessageResponse
        {
            TextKey = stringId,
            IsSuccess = true
        };
    }
    
    private double CalculateRank( string text )
    {
        if ( String.IsNullOrEmpty( text ) )
        {
            return 1;
        }

        int unAlphabetCharsCount = text.Count( x => !Char.IsLetter( x ) );

        return 1 - ((double) unAlphabetCharsCount / text.Length);
    }
}