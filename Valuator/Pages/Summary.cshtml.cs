using Infrastructure.Extensions;
using Infrastructure.RegionSharding;
using Integration.Redis.Client;
using Integration.Redis.ClientFactory;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages;

public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IRedisShardingClientFactory _redisShardingClientFactory;

    public SummaryModel( ILogger<SummaryModel> logger, IRedisShardingClientFactory redisShardingClientFactory )
    {
        _logger = logger;
        _redisShardingClientFactory = redisShardingClientFactory;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }
    public Country TextCountry { get; set; }

    public void OnGet( string id, Country country )
    {
        _logger.LogDebug( id );

        _logger.LogInformation( $"LOOKUP: {id}, {country.GetRegion()}" );

        IRedisClient redisClient = _redisShardingClientFactory.GetClient( country );
        if ( Double.TryParse( redisClient.Get( id.AsRankKey() ), out double rank ) )
        {
            Rank = rank;
        }

        if ( Double.TryParse( redisClient.Get( id.AsSimilarityKey() ), out double similarity ) )
        {
            Similarity = similarity;
        }

        TextCountry = country;
    }
}