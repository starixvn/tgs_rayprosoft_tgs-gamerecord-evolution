using Microsoft.Extensions.Options;
using Tgs.GameRecord.EvolutionService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.EvolutionService.Core.DispatchServices
{
    internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
    {
        private readonly ICommandService<EvolutionWorkerCommand> _commandService;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;
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
        }

        public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
        {
            var key = $"GameRecord:EvolutionService:{_gameRecordInfoOptions.GameProviderCode}:{command.Authorization}".ToLower();
            if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
            {
                if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
                {
                    try
                    {
                        var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                        var startTime = endTime.AddMinutes((double)_gameRecordInfoOptions.IntervalMinute * -1);

                        await _commandService.ExecuteAsync(new EvolutionWorkerCommand(
                             GameApiUrl: command.GameApiUrl,
                             Authorization: command.Authorization,
                             StartTime: startTime,
                             EndTime: endTime), cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        _ = await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
