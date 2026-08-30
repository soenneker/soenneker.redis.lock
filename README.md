[![](https://img.shields.io/nuget/v/Soenneker.Redis.Lock.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Lock/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.lock/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.redis.lock/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Redis.Lock.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Lock/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.lock/build-and-test.yml?label=build%20and%20test&style=for-the-badge)](https://github.com/soenneker/soenneker.redis.lock/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.lock/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.redis.lock/actions/workflows/codeql.yml)

# Soenneker.Redis.Lock

Provides expiring Redis locks with ownership-safe acquisition and release.

## Installation

```bash
dotnet add package Soenneker.Redis.Lock
```

## Registration

```csharp
using Soenneker.Redis.Lock.Registrars;

services.AddRedisLockUtilAsScoped();
```

Registration includes `Soenneker.Redis.Util`; configure its Redis connection before resolving `IRedisLockUtil`.

## Acquire a lock

```csharp
using Soenneker.Redis.Lock;
using Soenneker.Redis.Lock.Abstract;

RedisLockHandle? handle = await redisLocks.TryLock(
    "orders:42:fulfill",
    TimeSpan.FromSeconds(30),
    cancellationToken);

if (handle is null)
    return; // another owner holds the lock

await using (handle)
{
    await FulfillOrder(cancellationToken);
}
```

`TryLock` writes a unique ownership token only when the key does not exist. Disposing the handle removes the key only if that token still matches, so an expired lock acquired by another process is not released accidentally.

Choose an expiration longer than the protected operation. This package does not renew leases and does not issue fencing tokens; if an operation can outlive its lease, an old owner can continue running after a new owner acquires the key. For correctness-critical writes, add a fencing/version check in the protected resource.

## Lower-level operations

- `Check` is a point-in-time existence check, not proof that the caller owns a lock.
- `Lock` unconditionally writes the shared value `1`, overwriting any existing value. Prefer `TryLock` for mutual exclusion.
- `Unlock(lockName)` releases only a value written by `Lock`; `Unlock(lockName, lockValue)` performs a compare-and-delete.
- `ForceUnlock` and `ForceUnlockAll` delete without checking ownership and should be reserved for recovery/administration.

Cancellation can stop a pending call, but it does not undo a lock command Redis already accepted.
