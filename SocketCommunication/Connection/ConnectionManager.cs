using System.Net;
using System.Net.Sockets;

namespace SocketCommunication.Connection;

public static class ConnectionManager
{
    public static CustomSocketConnection ConnectToServer( IPAddress ipAddress, int port )
    {
        Socket sender = new Socket( ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp );

        sender.Connect( ipAddress, port );

        return new CustomSocketConnection( sender );
    }

    public static void StartListening( int port, Action<ReceivedData, CustomSocketConnection> packetHandler,
        CancellationToken cancellationToken = new CancellationToken() )
    {
        IPAddress ipAddress = IPAddress.Any;

        Socket listener = new Socket( ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp );

        listener.Bind( new IPEndPoint( ipAddress, port ) );

        listener.Listen( 10 );

        while ( !cancellationToken.IsCancellationRequested )
        {
            Socket handler = listener.Accept();
            using CustomSocketConnection customSocketConnection = new CustomSocketConnection( handler );

            ReceivedData receivedData = customSocketConnection.Receive();
            packetHandler.Invoke( receivedData, customSocketConnection );
        }
    }
}