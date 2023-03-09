namespace Tgs.GameRecord.EvolutionService.Core;

public interface IScheduleFormLock
{
    public Task<bool> LockingAsync(string gameProviderCode, long id);

    public Task<bool> UnlockAsync(string gameProviderCode, long id);
}
