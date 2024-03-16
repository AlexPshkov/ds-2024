namespace Integration.Nats.Messages.Implementation;

public class CalcMessageRequest : IEventMessage
{
    public string TextKey { get; set; }
}