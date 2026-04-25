using System.Collections.Generic;
using System.Threading;
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
    public async Task Check_after_lock_should_be_true(CancellationToken cancellationToken)
    {
        await _util.Lock("test", cancellationToken);

        bool locked = await _util.Check("test", cancellationToken);

        locked.Should().BeTrue();
    }

    [Test]
    public async Task Check_after_unlock_should_be_false(CancellationToken cancellationToken)
    {
        await _util.Lock("test", cancellationToken);

        await _util.Unlock("test", cancellationToken);

        bool locked = await _util.Check("test", cancellationToken);

        locked.Should().BeFalse();
    }

    [Test]
    public async Task UnlockAll_should_be_false_when_checking(CancellationToken cancellationToken)
    {
        await _util.Lock("test1", cancellationToken);
        await _util.Lock("test2", cancellationToken);

        await _util.UnlockAll(new List<string>{"test1", "test2"}, cancellationToken);

        bool locked1 = await _util.Check("test1", cancellationToken);
        bool locked2 = await _util.Check("test2", cancellationToken);

        locked1.Should().BeFalse();
        locked2.Should().BeFalse();
    }
}

