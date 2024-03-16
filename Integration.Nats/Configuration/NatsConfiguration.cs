namespace Integration.Nats.Configuration;

public class NatsConfiguration
{
    public static string SectionName = nameof( NatsConfiguration );
    
    public string NatsUrl { get; set; }
}