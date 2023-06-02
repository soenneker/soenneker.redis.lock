using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Soenneker.Redis.Lock.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Redis.Lock.Tests;

[Collection("RedisLockUtilCollection")]
public class RedisLockUtilTests : FixturedUnitTest
{
    private readonly IRedisLockUtil _util;

    public RedisLockUtilTests(RedisLockUtilFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
        _util = Resolve<IRedisLockUtil>(true);
    }

    [Fact]
    public async Task Check_after_lock_should_be_true()
    {
        await _util.Lock("test");

        bool locked = await _util.Check("test");

        locked.Should().BeTrue();
    }

    [Fact]
    public async Task Check_after_unlock_should_be_false()
    {
        await _util.Lock("test");

        await _util.Unlock("test");

        bool locked = await _util.Check("test");

        locked.Should().BeFalse();
    }

    [Fact]
    public async Task UnlockAll_should_be_false_when_checking()
    {
        await _util.Lock("test1");
        await _util.Lock("test2");

        await _util.UnlockAll(new List<string>{"test1", "test2"});

        bool locked1 = await _util.Check("test1");
        bool locked2 = await _util.Check("test2");

        locked1.Should().BeFalse();
        locked2.Should().BeFalse();
    }
}
