using System;
using System.Collections.Generic;
using Domain.Models;
using SocketCommunication.Connection;
using SocketCommunication.Packet.Implementation.Requests;
using SocketCommunication.Packet.Implementation.Responses;

namespace Server;

internal static class Program
{
    private static List<Message> Messages = new List<Message>();

    private static void Main( string[] args )
    {
        if ( args.Length != 1 || !Int32.TryParse( args[0], out int port ) )
        {
            Console.WriteLine( "Invalid arguments! Use: <port>" );
            return;
        }
        
        Console.WriteLine( $"Start listening messages on port {port}" );
        
        ConnectionManager.StartListening( port, ( receivedData, connection ) =>
        {
            switch ( receivedData.Type )
            {
                case nameof( SendMessageRequest ):
                    Handle( receivedData.Data<SendMessageRequest>(), connection );
                    return;                
                case nameof( ReceiveHistoryRequest ):
                    Handle( receivedData.Data<ReceiveHistoryRequest>(), connection );
                    return;
            }
        } );
    }

    private static void Handle( SendMessageRequest sendMessageRequest, CustomSocketConnection socketConnection )
    {
        Message message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            Text = sendMessageRequest.Message,
            Author = sendMessageRequest.Author,
            CreationTime = DateTime.Now
        };
        
        Messages.Add( message );
        
        Console.WriteLine($"Message '{message.Text}' received from {message.Author}");
        
        socketConnection.Send( new SendMessageResponse
        {
            IsSuccess = true,
            Message = message
        } );
    }
    
    private static void Handle( ReceiveHistoryRequest receiveHistoryRequest, CustomSocketConnection socketConnection )
    {
        socketConnection.Send( new ReceiveHistoryResponse
        {
            Messages = Messages
        } );
    }
}