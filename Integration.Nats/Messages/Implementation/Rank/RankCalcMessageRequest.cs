namespace Integration.Nats.Messages.Implementation.Rank;

public class RankCalcMessageRequest : IEventMessage
{
    public string TextKey { get; set; }
}