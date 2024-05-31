using System.Net;
using Polly;
using SocketCommunication.Connection;
using SocketCommunication.Packet.Implementation;

namespace Chain;

internal static class Program
{
    private static int _listeningPort;
    private static IPAddress _nextIpAddress;
    private static int _nextPort;
    
    private static bool _isInit;
    
    private static int? Value = null;
    
    public static void Main( string[] args )
    {
        _listeningPort = Int32.Parse( args[ 0 ] );
        _nextPort = Int32.Parse( args[ 2 ] );
        _isInit = false;
        
        string host = args[ 1 ] == "localhost" ? "127.0.0.1" : args[ 1 ]; // Финт ушами, чтобы избавиться от [::1]
        
        IPHostEntry ipHostEntry = Dns.GetHostEntry( host );
        IPAddress[] ipAddresses = ipHostEntry.AddressList;
        _nextIpAddress = ipAddresses.First();
        
        if ( args.Length >= 4 )
        {
            _isInit = Boolean.Parse( args[ 3 ] );
        }
        
        if ( args.Length >= 5 )
        {
            Value = Int32.Parse( args[ 4 ] );
        }

        Console.WriteLine($"Starts listening port {_listeningPort}. Next chain: {_nextIpAddress.ToString()}:{_nextPort}");

        if ( Value == null )
        {
            Console.Write( "Enter value: " );
            Value = Convert.ToInt32( Console.ReadLine() );
        }

        Policy
            .Handle<Exception>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(10), onRetry: (response, timespan, retryCount, context) =>
            {
                Console.WriteLine($"Attempt {retryCount} failed. Waiting {timespan} before next retry.");
            })
            .Execute( Start );

        Console.ReadKey();
    }

    private static void Start()
    {
        if ( _isInit )
        {
            WorkAsInitiator();
        }
        else
        {
            WorkAsProcess();
        } 
    }
    
    private static void WorkAsInitiator()
    {
        SendCurrentValueToNext();

        bool isFirstValueReceived = false;
        
        ConnectionManager.StartListening( _listeningPort, ( data, connection ) =>
        {
            NumberPacket numberPacket = data.Data<NumberPacket>();

            if ( !isFirstValueReceived )
            {
                Value = numberPacket.Number;
                isFirstValueReceived = true;
                SendCurrentValueToNext();
                return;
            }

            Value = numberPacket.Number;
            
            Console.WriteLine( "Max: " + Value );
        } );
    }

    private static void WorkAsProcess()
    {
        bool isFirstValueReceived = false;
        
        ConnectionManager.StartListening( _listeningPort, ( data, connection ) =>
        {
            NumberPacket numberPacket = data.Data<NumberPacket>();

            if ( !isFirstValueReceived )
            {
                isFirstValueReceived = true;
                SendMaxValueToNext( numberPacket.Number );
                return;
            }
            
            Value = numberPacket.Number;

            SendCurrentValueToNext();

            Console.WriteLine( "Max chain value: " + Value );
        } );
    }

    private static void SendCurrentValueToNext()
    {
        using CustomSocketConnection sender = ConnectionManager.ConnectToServer( _nextIpAddress, _nextPort );
        sender.Send( new NumberPacket { Number = Value!.Value } );
    }
    
    private static void SendMaxValueToNext( int receivedValue )
    {
        using CustomSocketConnection sender = ConnectionManager.ConnectToServer( _nextIpAddress, _nextPort );
        sender.Send( new NumberPacket { Number = Math.Max( receivedValue, Value!.Value ) } );
    }
}