namespace Tgs.GameRecord.EvolutionService.Core;

public interface IScheduleFormDispatcher
{
    ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default);
}
