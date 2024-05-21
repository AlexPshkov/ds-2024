using Newtonsoft.Json;

namespace SocketCommunication.Connection;

public class ReceivedData
{
    public string Type { get; set; }
    public string RawData { get; set; }
    
    public T Data<T>()
    {
        return JsonConvert.DeserializeObject<T>( RawData ) ?? throw new ArgumentException( "Empty data" );
    }
}