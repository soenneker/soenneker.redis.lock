using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Soenneker.Fixtures.Unit;
using Soenneker.Redis.Lock.Registrars;
using Soenneker.Utils.Test;

namespace Soenneker.Redis.Lock.Tests;

public class Fixture : UnitFixture
{
    public override async ValueTask InitializeAsync()
    {
        SetupIoC(Services);

        await base.InitializeAsync();
    }

    private static void SetupIoC(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: false);
        });

        IConfiguration config = TestUtil.BuildConfig();
        services.AddSingleton(config);
        services.AddRedisLockUtilAsSingleton();
    }
}