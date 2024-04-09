namespace Integration.Nats.Messages.Implementation;

public class RankCalcMessageRequest : IEventMessage
{
    public string TextKey { get; set; }
}