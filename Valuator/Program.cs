using Integration.Redis;
using Integration.Redis.Configuration;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        RedisClientConfiguration? redisClientConfiguration = builder.Configuration
            .GetSection(RedisClientConfiguration.SectionName)
            .Get<RedisClientConfiguration>();
        
        builder.Services.AddRedisClient( redisClientConfiguration );
        builder.Services.AddRedisDataProtection( redisClientConfiguration );
        
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
