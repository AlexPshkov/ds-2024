using Integration.Nats;
using Integration.Nats.Configuration;
using Integration.Redis;
using Integration.Redis.Configuration;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddRazorPages();

        RedisClientConfiguration? redisClientConfiguration = configuration.GetSection( RedisClientConfiguration.SectionName ).Get<RedisClientConfiguration>();
        NatsConfiguration? natsConfiguration = configuration.GetSection( NatsConfiguration.SectionName ).Get<NatsConfiguration>();

        builder.Services.AddRedisClient( redisClientConfiguration );
        builder.Services.AddRedisDataProtection( redisClientConfiguration );
        builder.Services.AddNatsClient( natsConfiguration );
        
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
