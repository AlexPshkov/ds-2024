namespace Infrastructure.Extensions;

public static class StringExtensions
{
    private const string RankKeyPrefix = "RANK-";
    private const string TextKeyPrefix = "TEXT-";
    private const string SimilarityKeyPrefix = "SIMILARITY-";
    
    public static string AsRankKey( this string id )
    {
        return $"{RankKeyPrefix}{id}";
    }
    
    public static string AsSimilarityKey( this string id )
    {
        return $"{SimilarityKeyPrefix}{id}";
    }    
    
    public static string AsTextKey( this string id )
    {
        return $"{TextKeyPrefix}{id}";
    }
    
    public static bool IsTextKey( this string id )
    {
        return id.StartsWith( TextKeyPrefix );
    }
}