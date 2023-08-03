using StackExchange.Redis;

namespace Tgs.GameRecord.Evolution.Core;

internal class ScheduleFormLock : IScheduleFormLock
{
    private readonly IDatabase _database;

    public ScheduleFormLock(
        IDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task<bool> LockingAsync(string gameProviderCode, long id)
        => _database.StringSetAsync(key: $"evo:scheduleform:{gameProviderCode}:{id}", value: id, expiry: TimeSpan.FromMinutes(1), when: When.NotExists);

    public Task<bool> UnlockAsync(string gameProviderCode, long id)
        => _database.KeyDeleteAsync(key: $"evo:scheduleform:{gameProviderCode}:{id}");
}
