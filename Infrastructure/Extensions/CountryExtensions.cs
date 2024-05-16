using Infrastructure.RegionSharding;
using Integration.Redis.RegionSharding;

namespace Infrastructure.Extensions;

public static class CountryExtensions
{
    public static Region GetRegion( this Country country )
    {
        switch (country)
        {
            case Country.Russia:
                return Region.RUS;
            case Country.France:
            case Country.Germany:
                return Region.EU;
            case Country.USA:
            case Country.India:
                return Region.OTHER;
            default:
                throw new ArgumentOutOfRangeException( nameof(country), country, $"No region for country {country}" );
        }
    }
}