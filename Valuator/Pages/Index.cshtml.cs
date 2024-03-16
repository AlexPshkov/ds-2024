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

        CalcMessageResponse? response = await _natsClient.MakeCalcRequest( new CalcMessageRequest
        {
            TextKey = stringId
        } );
        
        return Redirect( $"summary?id={response!.TextKey}" );
    }
}