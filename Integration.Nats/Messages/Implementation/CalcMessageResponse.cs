namespace Integration.Nats.Messages.Implementation;

public class CalcMessageResponse : IEventMessageResult
{
    public string TextKey { get; set; }
}