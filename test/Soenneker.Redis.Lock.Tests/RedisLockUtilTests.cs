using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Redis.Lock.Abstract;
using Soenneker.Tests.HostedUnit;


namespace Soenneker.Redis.Lock.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class RedisLockUtilTests : HostedUnitTest
{
    private readonly IRedisLockUtil _util;

    public RedisLockUtilTests(Host host) : base(host)
    {
        _util = Resolve<IRedisLockUtil>(true);
    }

    [Test]
    public async Task Check_after_lock_should_be_true()
    {
        await _util.Lock("test", CancellationToken);

        bool locked = await _util.Check("test", CancellationToken);

        locked.Should().BeTrue();
    }

    [Test]
    public async Task Check_after_unlock_should_be_false()
    {
        await _util.Lock("test", CancellationToken);

        await _util.Unlock("test", CancellationToken);

        bool locked = await _util.Check("test", CancellationToken);

        locked.Should().BeFalse();
    }

    [Test]
    public async Task UnlockAll_should_be_false_when_checking()
    {
        await _util.Lock("test1", CancellationToken);
        await _util.Lock("test2", CancellationToken);

        await _util.UnlockAll(new List<string>{"test1", "test2"}, CancellationToken);

        bool locked1 = await _util.Check("test1", CancellationToken);
        bool locked2 = await _util.Check("test2", CancellationToken);

        locked1.Should().BeFalse();
        locked2.Should().BeFalse();
    }
}
