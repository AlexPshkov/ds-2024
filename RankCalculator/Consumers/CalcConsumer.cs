using System.Globalization;
using Infrastructure.Extensions;
using Integration.Nats.Consumers;
using Integration.Nats.Messages;
using Integration.Nats.Messages.Implementation;
using Integration.Redis.Client;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace RankCalculator.Consumers;

public class CalcConsumer : BasicConsumer<CalcMessageRequest>
{
    private readonly IRedisClient _redisClient;
    private readonly ILogger<CalcConsumer> _logger;

    public CalcConsumer( IRedisClient redisClient, IConnection connection, ILogger<CalcConsumer> logger ) : base( connection )
    {
        _redisClient = redisClient;
        _logger = logger;
    }
    
    public override IEventMessageResult Handle( CalcMessageRequest calcMessageRequest )
    {
        string stringId = calcMessageRequest.TextKey;
        string text = _redisClient.Get( stringId.AsTextKey() );
        
        _redisClient.Save( stringId.AsRankKey(), CalculateRank( text ).ToString( CultureInfo.CurrentCulture ) );
        
        _redisClient.Save( stringId.AsSimilarityKey(), CalculateSimilarity( stringId.AsTextKey(), text ).ToString( CultureInfo.CurrentCulture ) );

        _logger.LogInformation( $"Successfully handled {calcMessageRequest.TextKey} by machine {@Environment.MachineName}" );
        
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
    
    private int CalculateSimilarity( string textKey, string text )
    {
        bool isSuch = _redisClient.GetAllKeys()
            .Any( x => x.IsTextKey() && x != textKey && _redisClient.Get( x ).Equals( text, StringComparison.CurrentCultureIgnoreCase ) );

        return isSuch ? 1 : 0;
    }
}