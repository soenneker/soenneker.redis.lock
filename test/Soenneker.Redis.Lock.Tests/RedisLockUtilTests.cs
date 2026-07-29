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
    private static readonly TimeSpan _lockExpiration = TimeSpan.FromMinutes(1);

    private readonly IRedisLockUtil _util;

    public RedisLockUtilTests(Host host) : base(host)
    {
        _util = Resolve<IRedisLockUtil>(true);
    }

    [Test]
    public async Task Check_after_lock_should_be_true(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        await _util.Lock(lockName, _lockExpiration, cancellationToken);

        bool locked = await _util.Check(lockName, cancellationToken);

        locked.Should().BeTrue();
    }

    [Test]
    public async Task Check_after_expiration_should_be_false(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        await _util.Lock(lockName, TimeSpan.FromMilliseconds(100), cancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);

        bool locked = await _util.Check(lockName, cancellationToken);

        locked.Should().BeFalse();
    }

    [Test]
    public async Task TryLock_should_not_overwrite_existing_lock(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        await using RedisLockHandle? firstHandle = await _util.TryLock(lockName, _lockExpiration, cancellationToken);
        await using RedisLockHandle? secondHandle = await _util.TryLock(lockName, _lockExpiration, cancellationToken);

        firstHandle.Should().NotBeNull();
        secondHandle.Should().BeNull();
    }

    [Test]
    public async Task TryLock_handle_should_release_owned_lock(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        RedisLockHandle? handle = await _util.TryLock(lockName, _lockExpiration, cancellationToken);
        handle.Should().NotBeNull();

        await handle!.DisposeAsync();

        bool locked = await _util.Check(lockName, cancellationToken);

        locked.Should().BeFalse();
    }

    [Test]
    public async Task Unlock_should_require_matching_lock_value(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        await _util.Lock(lockName, _lockExpiration, cancellationToken);

        bool mismatched = await _util.Unlock(lockName, "different", cancellationToken);
        bool stillLocked = await _util.Check(lockName, cancellationToken);
        bool matched = await _util.Unlock(lockName, "1", cancellationToken);

        mismatched.Should().BeFalse();
        stillLocked.Should().BeTrue();
        matched.Should().BeTrue();
    }

    [Test]
    public async Task ForceUnlock_should_remove_lock(CancellationToken cancellationToken)
    {
        string lockName = CreateLockName();

        await _util.Lock(lockName, _lockExpiration, cancellationToken);
        await _util.ForceUnlock(lockName, cancellationToken);

        bool locked = await _util.Check(lockName, cancellationToken);

        locked.Should().BeFalse();
    }

    [Test]
    public async Task ForceUnlockAll_should_remove_locks(CancellationToken cancellationToken)
    {
        string lockName1 = CreateLockName();
        string lockName2 = CreateLockName();

        await _util.Lock(lockName1, _lockExpiration, cancellationToken);
        await _util.Lock(lockName2, _lockExpiration, cancellationToken);

        await _util.ForceUnlockAll(new List<string> { lockName1, lockName2 }, cancellationToken);

        bool locked1 = await _util.Check(lockName1, cancellationToken);
        bool locked2 = await _util.Check(lockName2, cancellationToken);

        locked1.Should().BeFalse();
        locked2.Should().BeFalse();
    }

    private static string CreateLockName() => $"test:{Guid.NewGuid():N}";
}
