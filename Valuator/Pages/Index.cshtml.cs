using System.Globalization;
using Infrastructure.Extensions;
using Integration.Nats.Client;
using Integration.Nats.Messages.Implementation;
using Integration.Redis.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisClient _redisClient;
    private readonly INatsClient _natsClient;

    public IndexModel( ILogger<IndexModel> logger, IRedisClient redisClient, INatsClient natsClient )
    {
        _logger = logger;
        _redisClient = redisClient;
        _natsClient = natsClient;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost( string text )
    {
        _logger.LogDebug( text );
        
         Guid id = Guid.NewGuid();
         string stringId = id.ToString();

        _redisClient.Save( stringId.AsTextKey(), text );

        _redisClient.Save( stringId.AsSimilarityKey(), CalculateSimilarity( stringId.AsTextKey(), text ).ToString( CultureInfo.CurrentCulture ) );
        
        CalcMessageResponse? response = await _natsClient.MakeCalcRequest( new RankCalcMessageRequest
        {
            TextKey = stringId
        } );
        
        return Redirect( $"summary?id={response!.TextKey}" );
    }
    
    private int CalculateSimilarity( string textKey, string text )
    {
        bool isSuch = _redisClient.GetAllKeys()
            .Any( x => x.IsTextKey() && x != textKey && _redisClient.Get( x ).Equals( text, StringComparison.CurrentCultureIgnoreCase ) );

        return isSuch ? 1 : 0;
    }
}