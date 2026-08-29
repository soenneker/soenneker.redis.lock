[![](https://img.shields.io/nuget/v/Soenneker.Redis.Lock.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Lock/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.lock/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.redis.lock/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Redis.Lock.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Redis.Lock/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.redis.lock/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.redis.lock/actions/workflows/codeql.yml)

# Soenneker.Redis.Lock

A utility library leveraging Redis that provides distributed locking Typically Scoped IoC.

## Install

```bash
dotnet add package Soenneker.Redis.Lock
```

## Quick start

```csharp
using Soenneker.Redis.Lock.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddRedisLockUtilAsSingleton();
```

Registers Redis Lock Util with a singleton lifetime.

## What you get

- `IRedisLockUtil` — A utility library leveraging Redis that provides distributed locking Typically Scoped IoC.
- `RedisLockUtilRegistrar` — A utility library leveraging Redis that provides distributed locking.
- `RedisLockHandle` — Represents ownership of a distributed Redis lock acquired through `RedisLockUtil.TryLock`.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IRedisLockUtil.Check(lockName, cancellationToken)` | Checks if a Redis lock with the specified name is currently set. | `true` if the lock is currently set; otherwise `false`. |
| `IRedisLockUtil.TryLock(lockName, expiration, cancellationToken)` | Attempts to acquire a Redis lock only when it is not already set. | A handle that owns and releases the lock if acquisition succeeded; otherwise `null`. |
| `IRedisLockUtil.Lock(lockName, expiration, cancellationToken)` | Sets a Redis lock with the specified name and expiration. | A `ValueTask` representing the asynchronous operation. |
| `IRedisLockUtil.Unlock(lockName, cancellationToken)` | Releases a Redis lock created by `Lock`. | `true` if the lock was released; otherwise `false`. |
| `IRedisLockUtil.Unlock(lockName, lockValue, cancellationToken)` | Releases a Redis lock only when its current value matches the specified lock value. | `true` if the value matched and the lock was released; otherwise `false`. |
| `IRedisLockUtil.ForceUnlock(lockName, cancellationToken)` | Forcibly releases a Redis lock without checking its value. | A `ValueTask` representing the asynchronous operation. |
| `IRedisLockUtil.ForceUnlockAll(locks, cancellationToken)` | Forcibly releases multiple Redis locks without checking ownership. | A `Task` representing the asynchronous operation. |
| `RedisLockUtilRegistrar.AddRedisLockUtilAsSingleton(services)` | Registers Redis Lock Util with a singleton lifetime. | The same service collection, so additional registrations can be chained. |
| `RedisLockUtilRegistrar.AddRedisLockUtilAsScoped(services)` | Registers Redis Lock Util with a scoped lifetime. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
- Dispose instances you own when their scope ends so held resources can be released.
