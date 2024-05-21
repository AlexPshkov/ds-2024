using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using SocketCommunication.Packet;

namespace SocketCommunication.Connection;

public class CustomSocketConnection : IDisposable
{
    private const int BufferSize = 64;
    
    private readonly Socket _socket;

    public CustomSocketConnection( Socket socket )
    {
        _socket = socket;
    }
    
    public void Dispose()
    {
        _socket.Shutdown( SocketShutdown.Both );
        _socket.Close();
    }

    public T Send<T>( CustomSocketPacket packet )
    {
        Send( packet );
        
        return Receive().Data<T>();
    }
    
    public void Send( CustomSocketPacket packet )
    {
        _socket.Send( packet.ToBytes() );
    }
    
    public ReceivedData Receive()
    {
        byte[] buf = new byte[BufferSize];
                
        string data = String.Empty;
        
        ushort? contentLength = null;
        while ( contentLength == null || contentLength > data.Length )
        {
            int bytesRec = _socket.Receive(buf);

            if ( contentLength == null )
            {
                contentLength = BitConverter.ToUInt16( buf, 0 );
                data += Encoding.UTF8.GetString( buf, 2, bytesRec - 2 ); // Считали длину сообщения в формате ushort (первые два байта)
                continue;
            }
                    
            data += Encoding.UTF8.GetString( buf, 0, bytesRec );
        }

        CustomSocketPacket customSocketPacket = JsonConvert.DeserializeObject<CustomSocketPacket>( data ) ?? throw new ApplicationException( "Empty packet" );

        return new ReceivedData { Type = customSocketPacket.Name, RawData = data };
    }
}