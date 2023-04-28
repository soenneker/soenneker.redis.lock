using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Soenneker.Redis.Lock.Abstract;

/// <summary>
/// A utility library leveraging Redis that provides distributed locking <para/>
/// Typically Scoped IoC
/// </summary>
public interface IRedisLockUtil
{
    [Pure]
    ValueTask<bool> Check(string lockName);

    ValueTask Lock(string lockName);

    ValueTask Unlock(string lockName);

    /// <summary>
    /// Must remain Task
    /// </summary>
    Task UnlockAll(IEnumerable<string> locks);
}