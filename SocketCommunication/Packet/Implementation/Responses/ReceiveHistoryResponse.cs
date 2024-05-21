using Domain.Models;

namespace SocketCommunication.Packet.Implementation.Responses;

public class ReceiveHistoryResponse : CustomSocketPacket
{
    public List<Message> Messages { get; set; }
}