using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Domain.Models;
using SocketCommunication.Connection;
using SocketCommunication.Packet.Implementation.Requests;
using SocketCommunication.Packet.Implementation.Responses;

namespace Client;

internal static class Program
{
    private static readonly List<string> _names =
    [
        "Alexander",
        "Maria",
        "Dmitry",
        "Anna",
        "Sergey",
        "Elena",
        "Andrew",
        "Olga",
        "Ivan",
        "Tatiana"
    ];
    
    private static readonly List<string> _phrases =
    [
        "Hello, how are you?",
        "The weather is beautiful today.",
        "I love programming.",
        "Let's meet tomorrow.",
        "How was your day?",
        "I like this book.",
        "Have you seen the new movie?",
        "I have an idea.",
        "That's great news!",
        "What do you think about this?"
    ];

    public static void Main( string[] args )
    {
        if ( args.Length < 2 || !Int32.TryParse( args[1], out int port ) )
        {
            Console.WriteLine( "Invalid arguments! Use: <host> <port> [message]" );
            return;
        }

        string host = args[ 0 ] == "localhost" ? "127.0.0.1" : args[ 0 ]; // Финт ушами, чтобы избавиться от [::1]
        string message = args.Length < 3 || String.IsNullOrEmpty( args[2] ) ? GetRandomElement( _phrases ) : args[ 2 ];
        
        Console.WriteLine( $"Connecting to {host}:{port}..." );

        IPHostEntry ipHostEntry = Dns.GetHostEntry( host );
        IPAddress[] ipAddresses = ipHostEntry.AddressList;
        IPAddress ipAddress = ipAddresses.First();
        
        SendMessageResponse sendMessageResponse = SendMessage( ipAddress, port, message );
        Console.WriteLine( $"Message created: {sendMessageResponse.Message.Id}" );

        List<Message> messages = GetAllMessages( ipAddress, port );
        Console.WriteLine("Message history: \n - " + String.Join( "\n - ", messages.Select( x => x.Author + " : " + x.Text ) ) );
    }

    private static SendMessageResponse SendMessage( IPAddress ipAddress, int port, string message )
    {
        using CustomSocketConnection customSocketConnection = ConnectionManager.ConnectToServer( ipAddress, port );
        
        SendMessageResponse sendMessageResponse = customSocketConnection.Send<SendMessageResponse>( new SendMessageRequest
        {
            Author = GetRandomElement( _names ),
            Message = message
        } );

        return sendMessageResponse;
    }
    
    private static List<Message> GetAllMessages( IPAddress ipAddress, int port )
    {
        using CustomSocketConnection customSocketConnection = ConnectionManager.ConnectToServer( ipAddress, port );
        ReceiveHistoryResponse receiveHistoryResponse = customSocketConnection.Send<ReceiveHistoryResponse>( new ReceiveHistoryRequest() );

        return receiveHistoryResponse.Messages;
    }

    private static T GetRandomElement<T>( List<T> list )
    {
        Random random = new Random();
        int index = random.Next(list.Count);

        return list[index];
    }
}