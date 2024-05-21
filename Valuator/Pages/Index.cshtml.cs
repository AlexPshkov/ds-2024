using System.Globalization;
using Infrastructure.Extensions;
using Infrastructure.RegionSharding;
using Integration.Nats.Client;
using Integration.Nats.Messages.Implementation.Rank;
using Integration.Nats.Messages.Implementation.Similarity;
using Integration.Redis.Client;
using Integration.Redis.ClientFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisShardingClientFactory _redisShardingClientFactory;
    private readonly INatsClient _natsClient;

    public IndexModel( ILogger<IndexModel> logger, IRedisShardingClientFactory redisShardingClientFactory , INatsClient natsClient )
    {
        _logger = logger;
        _redisShardingClientFactory = redisShardingClientFactory ;
        _natsClient = natsClient;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost( string text, Country country )
    {
        if ( String.IsNullOrEmpty( text ) )
        {
            return new OkResult();
        } 
        
        _logger.LogDebug( text );
        
         Guid id = Guid.NewGuid();
         string stringId = id.ToString();

         _logger.LogInformation( $"LOOKUP: {id}, {country.GetRegion()}" );

         IRedisClient redisClient = _redisShardingClientFactory.GetClient( country );
         redisClient.Save( stringId.AsTextKey(), text );

         CalculateSimilarityAndSave( stringId, country );
         
        await _natsClient.MakeCalcRankRequest( new RankCalcMessageRequest
        {
            TextKey = stringId,
            Country = country
        } );
        
        return Redirect( $"summary?id={stringId}&country={country}" );
    }
    
    public void CalculateSimilarityAndSave( string stringId, Country country )
    {
        _logger.LogInformation( $"LOOKUP: {stringId}, {country.GetRegion()}" );
        
        IRedisClient redisClient = _redisShardingClientFactory.GetClient( country );
        int similarity = CalculateSimilarity( redisClient, stringId.AsTextKey() );
        
        redisClient.Save( stringId.AsSimilarityKey(), similarity.ToString( CultureInfo.CurrentCulture ) );

        _natsClient.Publish( new SimilarityCalculated
        {
            TextKey = stringId,
            Similarity = similarity
        } );
        
        _logger.LogInformation( $"Successfully handled {stringId} by machine {@Environment.MachineName}" );
    }
    
    private int CalculateSimilarity( IRedisClient redisClient, string textKey )
    {
        string text = redisClient.Get( textKey );
        
        bool isSuch = redisClient.GetAllKeys()
            .Any( x => x.IsTextKey() && x != textKey && redisClient.Get( x ).Equals( text, StringComparison.CurrentCultureIgnoreCase ) );

        return isSuch ? 1 : 0;
    }
}