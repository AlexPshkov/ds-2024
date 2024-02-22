namespace Integration.Redis.Client;

public interface IRedisClient
{
    List<string> GetAllKeys();
    string Get( string key );
    void Save( string key, string value );
}