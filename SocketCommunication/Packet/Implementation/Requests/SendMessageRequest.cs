namespace SocketCommunication.Packet.Implementation.Requests;

public class SendMessageRequest : CustomSocketPacket
{
    public string Author { get; set; }
    public string Message { get; set; }
}