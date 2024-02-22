using System.Globalization;
using Integration.Redis.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Extensions;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IRedisClient _redisClient;
    
    public IndexModel( ILogger<IndexModel> logger, IRedisClient redisClient )
    {
        _logger = logger;
        _redisClient = redisClient;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost( string text )
    {
        _logger.LogDebug( text );

        string id = Guid.NewGuid().ToString();

        _redisClient.Save( id.AsRankKey(), CalculateRank( text ).ToString( CultureInfo.CurrentCulture ) );
        
        _redisClient.Save( id.AsSimilarityKey(), CalculateSimilarity( text ).ToString( CultureInfo.CurrentCulture ) );
 
        _redisClient.Save( id.AsTextKey(), text );

        return Redirect( $"summary?id={id}" );
    }

    private double CalculateRank( string text )
    {
        if ( string.IsNullOrEmpty( text ) )
        {
            return 1;
        }

        int unAlphabetCharsCount = text.Count( x => !char.IsLetter( x ) );

        return 1 - (double) unAlphabetCharsCount / text.Length;
    }
    
    private int CalculateSimilarity( string text )
    {
        bool isSuch = _redisClient.GetAllKeys()
            .Any( x => x.IsTextKey() && _redisClient.Get( x ).Equals( text, StringComparison.CurrentCultureIgnoreCase ) );

        return isSuch ? 1 : 0;
    }
}