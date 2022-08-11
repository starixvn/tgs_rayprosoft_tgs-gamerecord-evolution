using Microsoft.Extensions.Options;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly ICommandService<EvolutionWorkerCommand> _commandService;
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly double _intervalMinute;
    private readonly IScheduleDispatch _scheduleDispatch;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        ICommandService<EvolutionWorkerCommand> commandService,
        IScheduleDispatch scheduleDispatch
        )
    {
        _gameRecordInfoOptions = options.Value;
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        _intervalMinute = (_gameRecordInfoOptions.IntervalMinute < _gameRecordInfoOptions.MinimumIntervalMinute)
                        ? _gameRecordInfoOptions.MinimumIntervalMinute
                        : _gameRecordInfoOptions.IntervalMinute;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"GameRecord:EvolutionService:{_gameRecordInfoOptions.GameProviderCode}:{command.CasinoId}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                try
                {
                    var endDate = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

                    var startDate = endDate.AddMinutes(_intervalMinute * -1);

                    await _commandService.ExecuteAsync(new EvolutionWorkerCommand(
                         GameApiUrl: command.GameApiUrl,
                         CasinoId: command.CasinoId,
                         GameHistoryApiToken: command.GameHistoryApiToken,
                         StartDate: startDate,
                         EndDate: endDate), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
