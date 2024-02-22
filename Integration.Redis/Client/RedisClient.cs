using StackExchange.Redis;

namespace Integration.Redis.Client;

public class RedisClient : IRedisClient
{
    private readonly IDatabase _redisDataBase;
    private readonly IServer _redisServer;
    
    public RedisClient( 
        IDatabase redisDataBase, 
        IServer redisServer )
    {
        _redisDataBase = redisDataBase;
        _redisServer = redisServer;
    }
    
    public List<string> GetAllKeys()
    {
        return _redisServer.Keys( _redisDataBase.Database )
            .Select( x => x.ToString() )
            .ToList();
    }

    public string Get( string key )
    {
        return _redisDataBase.StringGet( key ).ToString();
    }

    public void Save( string key, string value )
    {
        if ( !_redisDataBase.StringSet( key, value ) )
        {
            throw new ApplicationException( $"Can't set Key: {key} Value: {value} " );
        }
    }
}