namespace Integration.Nats.Messages.Implementation.Rank;

public class RankCalculated : IEventMessage
{
    public string TextKey { get; set; }
    public double Rank { get; set; }
}