using System;
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
        string lockName = CreateLockName();

        try
        {
            await _util.Lock(lockName, cancellationToken);

            bool locked = await _util.Check(lockName, cancellationToken);

            locked.Should().BeTrue();
        }
        finally
        {
            await _util.Unlock(lockName, CancellationToken.None);
        }
    }

    [Test]
    public async Task Check_after_unlock_should_be_false(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        await _util.Lock(lockName, cancellationToken);

        await _util.Unlock(lockName, cancellationToken);

        bool locked = await _util.Check(lockName, cancellationToken);

        locked.Should().BeFalse();
    }

    [Test]
    public async Task UnlockAll_should_be_false_when_checking(CancellationToken cancellationToken)
    {
        string lockName1 = CreateLockName();
        string lockName2 = CreateLockName();

        await _util.Lock(lockName1, cancellationToken);
        await _util.Lock(lockName2, cancellationToken);

        await _util.UnlockAll(new List<string> { lockName1, lockName2 }, cancellationToken);

        bool locked1 = await _util.Check(lockName1, cancellationToken);
        bool locked2 = await _util.Check(lockName2, cancellationToken);

        locked1.Should().BeFalse();
        locked2.Should().BeFalse();
    }

    private static string CreateLockName() => $"test:{Guid.NewGuid():N}";
}

