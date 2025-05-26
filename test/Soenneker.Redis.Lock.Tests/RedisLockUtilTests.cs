using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Redis.Lock.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Redis.Lock.Tests;

[Collection("Collection")]
public class RedisLockUtilTests : FixturedUnitTest
{
    private readonly IRedisLockUtil _util;

    public RedisLockUtilTests(Fixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        _util = Resolve<IRedisLockUtil>(true);
    }

    [Fact]
    public async Task Check_after_lock_should_be_true()
    {
        await _util.Lock("test", CancellationToken);

        bool locked = await _util.Check("test", CancellationToken);

        locked.Should().BeTrue();
    }

    [Fact]
    public async Task Check_after_unlock_should_be_false()
    {
        await _util.Lock("test", CancellationToken);

        await _util.Unlock("test", CancellationToken);

        bool locked = await _util.Check("test", CancellationToken);

        locked.Should().BeFalse();
    }

    [Fact]
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
