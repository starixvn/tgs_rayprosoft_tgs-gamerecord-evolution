using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Evolution.Core.JobApplicationServices;

internal class JobExecuteCommandService : ICommandService<JobExecuteCommand>
{
    private readonly IGameRoundsScheduleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly double _intervalMinutes;
    private readonly ILogger<JobExecuteCommandService> _logger;

    public JobExecuteCommandService(
        ILogger<JobExecuteCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IGameRoundsScheduleClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordInfo = options.Value;
        _intervalMinutes = _gameRecordInfo.IntervalMinutesMax < _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMax
            : _gameRecordInfo.IntervalMinutesMinimum > _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMinimum
            : _gameRecordInfo.IntervalMinutes;
    }

    public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
        var startTime = endTime.AddMinutes(_intervalMinutes * -1);

        var parameters = new
        {
            command.ApiUrl,
            command.CasinoId,
            command.GameHistoryApiToken,
            StartTime = startTime,
            EndTime = endTime
        };

        _logger.LogInformation("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

        return _client
            .Evolution_ScheduleFormQueuedAsync(
            gameProviderCode: command.GameProviderCode,
            name: command.Name,
            subject: "gp.gamerecord.scheduleform.evotf.queued",
            source: ScheduleFormSource.Schedule,
            apiUrl: parameters.ApiUrl,
            casinoId: parameters.CasinoId,
            gameHistoryApiToken: parameters.GameHistoryApiToken,
            startTime: parameters.StartTime,
            endTime: parameters.EndTime,
            cancellationToken: cancellationToken);
    }
}
