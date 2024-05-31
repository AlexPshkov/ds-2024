using Domain.Models;

namespace SocketCommunication.Packet.Implementation.Responses;

public class SendMessageResponse : CustomSocketPacket
{
    public bool IsSuccess { get; set; }
    public Message Message { get; set; }
}