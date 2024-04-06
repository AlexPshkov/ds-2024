namespace Integration.Nats.Messages.Implementation.Similarity;

public class SimilarityCalcMessageRequest : IEventMessage
{
    public string TextKey { get; set; }
}