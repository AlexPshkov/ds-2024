namespace Integration.Nats.Messages.Implementation;

public class CommonResponse : IEventMessageResult
{
    public bool IsSuccess { get; set; }
}