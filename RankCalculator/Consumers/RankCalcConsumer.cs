using System.Globalization;
using Infrastructure.Extensions;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Redis.Client;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace RankCalculator.Consumers;

public class RankCalcConsumer : BasicConsumer<RankCalcMessageRequest>
{
    private readonly IRedisClient _redisClient;
    private readonly ILogger<RankCalcConsumer> _logger;

    public RankCalcConsumer( IRedisClient redisClient, IConnection connection, ILogger<RankCalcConsumer> logger ) : base( connection )
    {
        _redisClient = redisClient;
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( RankCalcMessageRequest rankCalcMessageRequest )
    {
        string stringId = rankCalcMessageRequest.TextKey;
        string text = _redisClient.Get( stringId.AsTextKey() );
        
        _redisClient.Save( stringId.AsRankKey(), CalculateRank( text ).ToString( CultureInfo.CurrentCulture ) );

        _logger.LogInformation( $"Successfully handled {rankCalcMessageRequest.TextKey} by machine {@Environment.MachineName}" );
        
        return new CalcMessageResponse
        {
            TextKey = stringId
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