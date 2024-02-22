using Integration.Redis.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Extensions;

namespace Valuator.Pages;

public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IRedisClient _redisClient;

    public SummaryModel( ILogger<SummaryModel> logger, IRedisClient redisClient )
    {
        _logger = logger;
        _redisClient = redisClient;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet( string id )
    {
        _logger.LogDebug( id );

        if ( double.TryParse( _redisClient.Get( id.AsRankKey() ), out double rank ) )
        {
            Rank = rank;
        }

        if ( double.TryParse( _redisClient.Get( id.AsSimilarityKey() ), out double similarity ) )
        {
            Similarity = similarity;
        }
    }
}