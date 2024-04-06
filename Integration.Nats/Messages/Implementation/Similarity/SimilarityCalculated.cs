namespace Integration.Nats.Messages.Implementation.Similarity;

public class SimilarityCalculated : IEventMessage
{
    public string TextKey { get; set; }
    public int Similarity { get; set; }
}