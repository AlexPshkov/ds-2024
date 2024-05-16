using Infrastructure.RegionSharding;

namespace Integration.Nats.Messages.Implementation.Rank;

public class RankCalcMessageRequest : IEventMessage
{
    public string TextKey { get; set; }
    public Country Country { get; set; }
}